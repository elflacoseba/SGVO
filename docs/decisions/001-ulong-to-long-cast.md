# ADR-001: Cast de ulong (MySQL) a long (C# API)

## Estado
Aceptado

## Contexto
La base de datos MySQL utiliza `BIGINT UNSIGNED` para las claves primarias y foráneas, lo que en C# se mapea a `ulong` (UInt64, rango 0 a 18,446,744,073,709,551,615).

Sin embargo, los DTOs de la API y los endpoints utilizan `long` (Int64, rango -9,223,372,036,854,775,808 a 9,223,372,036,854,775,807).

## Decisión
Realizar un cast explícito de `ulong` a `long` en los query handlers al mapear entidades de dominio a DTOs:

```csharp
.Select(e => new SomeDto
{
    Id = (long)e.Id,  // ulong → long
    // ...
})
```

## Justificación

### Por qué no mantener ulong en la API

1. **Compatibilidad JSON**: JavaScript/TypeScript (el cliente más común) no soporta nativamente `ulong`. JSON.parse() puede perder precisión para valores > 2^53 - 1 (9,007,199,254,740,991).

2. **OpenAPI/Swagger**: El estándar OpenAPI no distingue entre signed/unsigned integers. `ulong` se representa como `integer` con `format: int64`, lo que puede causar confusión en clientes generados automáticamente.

3. **Ecosistema .NET**: Muchas librerías de serialización, validación y mapeo esperan `long` por defecto. Usar `ulong` requiere configuración adicional.

4. **Consistencia con estándares REST**: La mayoría de APIs REST públicas usan `long` para IDs, incluso cuando el almacenamiento subyacente es unsigned.

### Por qué no cambiar MySQL a BIGINT SIGNED

1. **Rango positivo suficiente**: `BIGINT SIGNED` va hasta 9,223,372,036,854,775,807. Para SGVO, este rango es más que suficiente (no esperamos superar 9 quintillones de registros).

2. **Migración costosa**: Cambiar todas las columnas de `UNSIGNED` a `SIGNED` requeriría:
   - Modificar todas las tablas (PKs y FKs)
   - Re-escribir los scripts de migración SQL
   - Riesgo de corrupción de datos en producción

3. **Sin beneficio real**: No hay casos de uso donde necesitemos valores negativos para IDs.

### Riesgos del cast

1. **Overflow silencioso**: Si un ID supera `long.MaxValue` (9.2 quintillones), el cast resultará en un valor negativo. Esto es extremadamente improbable en la práctica.

2. **Validación en API**: Los clientes podrían enviar IDs negativos. Se debe validar que `id > 0` en los endpoints.

## Mitigaciones

1. **Validación en controladores**: Usar `[Range(1, long.MaxValue)]` o validación manual para rechazar IDs negativos.

2. **Documentación**: Este ADR documenta la decisión para futuros desarrolladores.

3. **Tests**: Los tests unitarios verifican que el mapeo funciona correctamente para IDs válidos.

## Consecuencias

### Positivas
- API compatible con JavaScript/TypeScript sin pérdida de precisión
- OpenAPI/Swagger genera clientes correctos automáticamente
- Consistencia con el ecosistema .NET y estándares REST
- Sin cambios en la base de datos

### Negativas
- Cast explícito en cada query handler (código boilerplate)
- Riesgo teórico de overflow (mitigado por validación)
- Posible confusión inicial para desarrolladores nuevos (mitigado por este ADR)

## Referencias
- [MySQL BIGINT](https://dev.mysql.com/doc/refman/8.0/en/integer-types.html)
- [JavaScript Number.MAX_SAFE_INTEGER](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Number/MAX_SAFE_INTEGER)
- [OpenAPI Schema Types](https://swagger.io/docs/specification/data-models/data-types/)
