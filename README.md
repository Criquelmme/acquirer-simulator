# Mock Adquirente

Minimal API en .NET 8 que simula un adquirente bancario para probar la resiliencia de un procesador de pagos.

## Endpoint

### `POST /authorize`

Recibe un payload JSON con el campo `amount` (entero) y responde según el **último dígito** del monto.

#### Payload de ejemplo

```json
{
  "amount": 1000
}
```

#### Comportamiento según el último dígito

| Dígito | Código HTTP | Respuesta | Descripción |
|--------|-------------|-----------|-------------|
| 0      | `200 OK`    | `{ "auth_code": "AB12CD", "status": "APPROVED" }` | Caso feliz — autorización aprobada. |
| 1      | `400 Bad Request` | `{ "error": "INVALID_CARD_OR_FUNDS" }` | Falla crítica irreversible — tarjeta inválida o sin fondos. |
| 2      | `504 Gateway Timeout` | *(sin cuerpo)* | Falla por timeout — espera 30 segundos antes de responder. |
| 3      | `503 Service Unavailable` | *(sin cuerpo)* | Falla temporal del servidor. |
| Otro   | `200 OK`    | `{ "auth_code": "AB12CD", "status": "APPROVED" }` | Comportamiento por defecto igual al caso feliz. |

## Cómo ejecutar

```bash
dotnet run
```

La API escucha en `http://localhost:5001`.

Swagger UI disponible en `http://localhost:5001/swagger`.

## Propósito

Este mock permite simular distintos escenarios de falla (crítica, timeout, servidor caído) para validar que un procesador de pagos implemente correctamente patrones de resiliencia como **retry**, **circuit breaker** y **timeout handling**.
