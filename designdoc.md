#DesignDoc

## Motivación:
Endpoints de API que generen comprobantes de venta y los autoricen en el ente tributario de Argentina, ARCA en adelante. La prioridad es la simplicidad y garantizar la correcta numeración consecutiva, incluso bajo alta concurrencia o ante caídas de ARCA (Algo que lamentablemente es muy común). La idea es hacer un endpoint por tipo de comprobante. Además, se busca implementar una arquitectura multi-empresa.

## Contexto:
Funcionamiento general de ARCA y los comprobantes en Argentina. Los datos más importantes de un comprobante de venta son:

**Tipo de comprobante:** Factura, Factura MiPyme, Nota de crédito, etc.

**Punto de venta:** Identificador numérico que se le asigna a la caja o sucursal que emite el comprobante.

**Número:** Número correlativo que identifica a un comprobante autorizado de un determinado punto de venta.

Al intentar autorizar un comprobante del tipo **X**, punto de venta **Y**, y número **Z**, éste puede resultar autorizado o rechazado.

Si es autorizado, ARCA le asigna un CAE (Código de Autorización Electrónico) y una fecha de vencimiento a ese CAE. Además, ARCA guarda la numeración que le dimos, por lo que el siguiente comprobante del tipo **X** y punto de venta **Y** a autorizar debe ser obligatoriamente el número **Z + 1**.

Si es rechazada, ARCA nos informa el motivo, pero no guarda el número **Z** como rechazado. Es decir, el próximo comprobante del tipo **X** y punto de venta **Y** a autorizar sí o sí debe ser **Z**.

## Dependencias del servicio:
Este servicio tiene una dependencia de tiempo de ejecución (síncrona) con los siguientes servicios para la recolección de datos antes de la autorización:
`clients-service:` Para obtener los datos fiscales del cliente.
`products-service:` Para obtener los datos fiscales de los productos a facturar.

Se toma la decisión de que la llamada sea a estos servicios sea sincrónica para priorizar la consistencia de los datos. Se prefiere el acoplamiento a facturar con datos desactualizados.

## Autenticación
El Invoicing service estará segurizado por un JWT. Existirá un identity-service será emisor de ese JWT, que deberá tener claims tales como el TenantId y UserId.

## Alternativas descartadas:

**A)** Se había pensado en primera instancia no utilizar una tabla de numeración, sino que hacer un SELECT MAX(Número) + 1 simplemente en la tabla del comprobante a autorizar. Se descartó esta propuesta por no ser muy segura en entornos de alta concurrencia. 
**Ejemplo:** 

**1)** El usuario 1 y 2 intentan autorizar una factura para el mismo POS al mismo tiempo.

**2)** El operador 1 obtiene el número 4 al hacer SELECT MAX(Número) + 1 FROM INVOICES.

**3)** El operador 2 obtiene el número 4 al hacer SELECT MAX(Número) + 1 FROM INVOICES.

**4)** La factura del usuario 1 se autoriza en ARCA.

**5)** La factura del usuario 2 es rechazada en ARCA por numeración corrupta. Lo cual generará una mala UX.

**B)** Arquitectura de colas. Que la API `POST /invoices` (o cualquier otro tipo de comprobante) no autorice, sino que solo guarde una "solicitud" en una cola de Service Bus. Un worker (Azure Function) procesaría la cola. Si bien este patrón ofrece el mejor rendimiento y elimina los bloqueos de base de datos, fue descartado por priorizar la simplicidad del proyecto. Esta arquitectura introduce una complejidad de orden superior para el cliente (la UI), que ahora tendría que manejar una respuesta asíncrona (ya no recibe la factura al instante), requiriendo un sistema de notificaciones (como SignalR) que está fuera del alcance de este proyecto. Se opta por una solución síncrona más simple pero atómicamente correcta.

## Propuesta:

Se proponen endpoints síncronos por cada tipo de comprobante (POST /invoices, POST /credit-invoice, POST /credit-notes, POST /debit-notes) que garantizan la integridad de los datos mediante un flujo de ejecución que prioriza la atomicidad y la seguridad por sobre el rendimiento.

El flujo de autorización seguirá la siguiente secuencia:
**1) Chequeo de idempotencia:** La solicitud de autorización debe primero pasar un chequeo de idempotencia. Se intentará insertar la `Idempotency-Key` (provista en la cabecera de la petición) en la tabla AuthorizationRequest con estado `Pending`.
Si el insert falla por violación de PK significa que esa solicitud ya fue procesada y retornará la respuesta cacheada (`200 OK` si fue autorizada exitosamente, `409 Conflict` si está pendiente, `400 Bad Request` si la solicitud de autorización fue rechazada).  

**2) Recolección de datos y cálculos:** Solamente si la `Idempotency-Key` es nueva, el servicio recolectará los datos necesarios para la autorización. Esto implica llamadas síncronas a los microservicios `clients-service` y `products-service` para obtener los datos fiscales del cliente y los productos respectivamente. Se acepta este acoplamiento para garantizar la consistencia de los datos, es decir, evitar facturar con precios obsoletos o categorías fiscales obsoletas. Con estos datos, el motor de impuestos de `invoicing-service` consultará la matriz de reglas (TaxRules) y calculará los impuestos aplicables al comprobante y sus totales.

**3) Transacción:** Si los pasos 1 y 2 son exitosos, el servicio inciará la transacción en la base de datos para la autorización.
Se bloqueará la fila en `PointOfSaleCounters` correspondiente al tipo de comprobante del PointOfSale que estamos intentando autorizar. El bloqueo será usando `WITH (UPDLOCK, ROWLOCK)`, de esta forma podremos tomar el numero que corresponda autorizar de forma segura, es decir, no pueden haber dos procesos de autorización que utilicen el mismo numero para un mismo tipo de comprobante de un mismo PointOfSale.
Se llamará a ARCA con los totales calculados y un numero válido para la autorización. Esto es pésimo en rendimiento, ya que si la llamada a ARCA tarda 10 segundos, tendré la tabla `PointOfSaleCounters` bloqueada para ese tipo de comprobante y PointOfSale por al menos 10 segundos. Nuevamente, se toma la decisión de preservar la integridad de los datos aceptando un rendimiento menor.
Si el intento de autorización es rechazado por ARCA, la transacción se revertirá. Esto liberá el registro bloqueado de `PointOfSaleCounters` sin corromper la numeración. No se guardará ningún dato en la tabla del tipo de comprobante y en la tabla Requests del comprobante se marcará el intento de autorización como `Failed`. Se devolverá un `400 Bad Request` al usuario.
Si el comprobante es autorizado por ARCA, se hará commit de la transacción, guardando el comprobante en su tabla, con su correspondiente desglose de items en su tabla de items y guardando  el desglose de impuestos en la tabla de impuestos relacionados a los impuestos a nivel item. Se actualizará el campo `LastNumber` del registro que corresponda de `PointOfSaleCounters`. 
Si ARCA está caído se ejecutará inmediatamente un ROLLBACK de la transacción y se devolverá un `503 Service Unavaible`.

**4) Publicación de eventos:** Tras el commit exitoso, el servicio actualizará la tabla de requests con un status `Success` y publicará un evento en Azure Service Bus, anunciando la autorización del comprobante. Esto desacopla los procesos de fondo como la generación del PDF del comprobante autorizado o la actualización de cuentas corrientes.

**5) Respuesta:** El servicio devolverá un `201 Created` al cliente con el JSON del comprobante autorizado. 

En lo que sigue, se hará hincapié en la autorización de facturas. En próximas iteraciones se agregarán el resto de comprobantes.

## Diseño de datos:

### 1) Tabla PointOfSales
#### Rol: Gestionar los PointOfSales 
- Id (UNIQUEIDENTIFIER, PRIMARY KEY)
- Number (INT, NOT NULL, UNIQUE)
- Name (NVARCHAR)
- IsDisabled (BIT, NOT NULL, DEFAULT 0)
- TenantId (UNIQUEIDENTIFIER, NOT NULL)

### 2) Tabla PointOfSalesCounters
#### Rol: Garantizar la correcta numeración consecutiva para cada PointOfSales. Es la tabla crítica del sistema, diseñada para bloquear su actualización a nivel fila cuando un comprobante de un determinado PointOfSale está en proceso de autorización.
- PointOfSaleId (UNIQUEIDENTIFIER, PRIMARY KEY, FK a PointOfSales.Id)
- ArcaVoucherCode (INT, PRIMARY KEY)
- LastNumber (BIGINT, NOT NULL)
- LastIssueDate (DATETIMEOFFSET, NOT NULL)
- TenantId (UNIQUEIDENTIFIER, NOT NULL)

### 3) Tabla AuthorizationRequest
#### Rol: Garantizar la idempotencia de una solicitud de autorización y llevar una bitácora de cada intento.
- IdempotencyKey (UNIQUEIDENTIFIER, PRIMARY KEY)
- PointOfSaleId (UNIQUEIDENTIFIER, NOT NULL, FK a PointOfSales.Id)
- Status (VARCHAR(20), NOT NULL)
- RequestPayload (NVARCHAR(MAX), NOT NULL)
- FailureReason (NVARCHAR(MAX))
- UserId (UNIQUEIDENTIFIER, NOT NULL)
- CreatedAtUtc (DATETIME2, NOT NULL, DEFAULT GETUTCDATE())
- AuthorizedDocumentId (UNIQUEIDENTIFIER) -- Columna polimórfica
- ArcaVoucherCode (INT, NOT NULL)
- DocumentDescription (VARCHAR(50), NOT NULL)
- TenantId (UNIQUEIDENTIFIER, NOT NULL)

### 4) Tabla Invoices
#### Rol: Acta legal, solo facturas autorizadas
- Id (UNIQUEIDENTIFIER, PRIMARY KEY)
- ClientId (UNIQUEIDENTIFIER, NOT NULL)
- PointOfSaleId (UNIQUEIDENTIFIER, NOT NULL, FK a PointOfSales.Id)
- TenantId (UNIQUEIDENTIFIER, NOT NULL)
- CreatedByUserId (UNIQUEIDENTIFIER, NOT NULL) - El OperatorId que la creó.
- InvoiceNumber (BIGINT, NOT NULL)
- InvoiceLetter (CHAR(1), NOT NULL)
- ArcaVoucherCode (TinyInt, NOT NULL)
- Cae (VARCHAR(14), NOT NULL)
- CaeDueDate (DATETIMEOFFSET, NOT NULL)
- TotalAmount (DECIMAL(18, 2), NOT NULL)
- NetAmount (DECIMAL(18, 2), NOT NULL)
- TaxAmount (DECIMAL(18, 2), NOT NULL)
- Currency (CHAR(3), NOT NULL)
- PaymentCurrency (CHAR(3), NOT NULL)
- ExchangeRate (DECIMAL(18, 4), NOT NULL)
- IssueDate (DATETIMEOFFSET, NOT NULL)
- DueDate (DATETIMEOFFSET, NOT NULL)
- CreatedAtUtc (DATETIME2, NOT NULL, DEFAULT GETUTCDATE())

Constraints:
- UQ_InvoiceNumber UNIQUE (TenantId, PointOfSaleId, InvoiceNumber, ArcaVoucherCode)

### 5) Tabla InvoiceItems
#### Rol: Items de una factura autorizada
- Id (UNIQUEIDENTIFIER, PRIMARY KEY)
- InvoiceId (UNIQUEIDENTIFIER, NOT NULL, FK a Invoices.Id ON DELETE CASCADE)
- ProductId (UNIQUEIDENTIFIER, NOT NULL)
- TenantId (UNIQUEIDENTIFIER, NOT NULL)
- Description (NVARCHAR(255), NOT NULL)
- Quantity (DECIMAL(18, 4), NOT NULL)
- UnitPrice (DECIMAL(18, 4), NOT NULL)
- Subtotal (DECIMAL(18, 2), NOT NULL)
- TaxAmount (DECIMAL(18, 2), NOT NULL)

### 6) Tabla Taxes
#### Rol: Orquestadora del motor de impuestos. Define un impuesto.
- Id (UNIQUEIDENTIFIER, PRIMARY KEY)
- TenantId (UNIQUEIDENTIFIER, NOT NULL)
- Name (NVARCHAR(100), NOT NULL) - Ejemplo: 'IVA 21%'
- Rate (DECIMAL(18, 6), NOT NULL) - Ejemplo: 0.21

### 7) Tabla TaxRules
#### Rol: Matriz de impuestos. Es para poder decidir que impuesto corresponde en base a los datos fiscales del producto y el receptor.
- Id (UNIQUEIDENTIFIER, PRIMARY KEY)
- TenantId (UNIQUEIDENTIFIER, NOT NULL)
- ProductTaxCategoryId (TINYINT, NOT NULL)
- ClientTaxCategoryId (TINYINT, NOT NULL)
- TaxId (UNIQUEIDENTIFIER, NOT NULL, FK a Taxes)

### 8) Tabla InvoiceItemTaxes
#### Rol: Desglose de impuestos para cada item de la factura.
- Id (UNIQUEIDENTIFIER, PRIMARY KEY)
- TenantId (UNIQUEIDENTIFIER, NOT NULL)
- InvoiceItemId (UNIQUEIDENTIFIER, NOT NULL, FK a InvoiceItems)
- TaxId (UNIQUEIDENTIFIER, NOT NULL, FK a Taxes)
- TaxName (NVARCHAR(100), NOT NULL)
- TaxRate (DECIMAL(18, 6), NOT NULL)
- BaseAmount (DECIMAL(18, 2), NOT NULL)
- AppliedAmount (DECIMAL(18, 2), NOT NULL)


## Contrato de API:
`/invoices POST`

Autoriza una nueva factura de venta de forma síncrona. Este endpoint orquesta la validación de negocio, el cálculo de impuestos y la transacción atómica con el sistema de numeración y el ente tributario (ARCA).

### Headers

| Header | Tipo | Requerido | Descripción |
| :--- | :--- | :--- | :--- |
| `Authorization` | `string` | **Sí** | Token JWT (Bearer \<token\>). Se utilizará para extraer el TenantId y UserId |
| `Idempotency-Key` | `GUID` | **Sí** | Clave única generada por el cliente para garantizar que no se dupliquen operaciones ante reintentos de red. |

### Body

| Campo | Tipo | Requerido | Descripción |
| :--- | :--- | :--- | :--- |
| `clientId` | `GUID` | **Sí** | ID del cliente receptor (referencia a `clients-service`) |
| `pointOfSaleId` | `GUID` | **Sí** | ID del punto de venta emisor |
| `issueDate` | `string` (ISO 8601) | **Sí** | Fecha de emisión del comprobante |
| `dueDate` | `string` (ISO 8601) | **Sí** | Fecha de vencimiento del pago |
| `currency` | `string` (3 chars) | **Sí** | Código ISO de la moneda |
| `paymentCurrency` | `string` (3 chars) | **Sí** | Moneda en la que se cancelará el comprobante |
| `exchangeRate` | `decimal` | **Sí** | Cotización de la moneda. Este campo es ignorado si la moneda es `ARS` |
| `items` | `Array<InvoiceItem>` | **Sí** | Lista de al menos un ítem |

#### InvoiceItem

| Header | Tipo | Requerido | Descripción |
| :--- | :--- | :--- | :--- |
| `productId` | `GUID` | **Sí** | ID del producto (referencia a `products-service`) |
| `quantity` | `decimal` | **Sí** | Cantidad a facturar |
| `unitPrice` | `decimal` | **No** | Precio unitario del producto. Si no se envia, se utiliza el precio por defecto del producto |
| `description` | `string` | **No** | Descripción del item facturado. Si no se envia, se utiliza el nombre del producto |

### Validaciones