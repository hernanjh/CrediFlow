using CrediFlow.Domain.Entities;
using CrediFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrediFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ParametricasController : ControllerBase
{
    private readonly CrediFlowDbContext _ctx;

    public ParametricasController(CrediFlowDbContext ctx)
    {
        _ctx = ctx;
    }

    // ─── CONDICIONES IVA ──────────────────────────────────────────────────
    [HttpGet("condiciones-iva")]
    public async Task<IActionResult> GetCondicionesIva([FromQuery] bool soloActivas = true)
    {
        var q = _ctx.CondicionesIva.AsQueryable();
        if (soloActivas) q = q.Where(x => x.Activa);
        var res = await q.Select(x => new { x.Id, x.Nombre, x.Activa }).ToListAsync();
        return Ok(res);
    }

    [HttpPost("condiciones-iva")]
    public async Task<IActionResult> CreateCondicionIva([FromBody] CondicionIvaRequest req)
    {
        var c = CondicionIva.Crear(req.Nombre);
        _ctx.CondicionesIva.Add(c);
        await _ctx.SaveChangesAsync();
        return Ok(new { c.Id });
    }

    [HttpPut("condiciones-iva/{id}")]
    public async Task<IActionResult> UpdateCondicionIva(Guid id, [FromBody] CondicionIvaRequest req)
    {
        var c = await _ctx.CondicionesIva.FindAsync(id);
        if (c == null) return NotFound();
        c.Actualizar(req.Nombre);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("condiciones-iva/{id}")]
    public async Task<IActionResult> DeleteCondicionIva(Guid id)
    {
        var c = await _ctx.CondicionesIva.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(false);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("condiciones-iva/{id}/reactivar")]
    public async Task<IActionResult> ReactivarCondicionIva(Guid id)
    {
        var c = await _ctx.CondicionesIva.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(true);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    // ─── CONDICIONES PAGO ─────────────────────────────────────────────────
    [HttpGet("condiciones-pago")]
    public async Task<IActionResult> GetCondicionesPago([FromQuery] bool soloActivas = true)
    {
        var q = _ctx.CondicionesPago.AsQueryable();
        if (soloActivas) q = q.Where(x => x.Activa);
        var res = await q.Select(x => new { x.Id, x.Nombre, x.DiasPlazo, x.Activa }).ToListAsync();
        return Ok(res);
    }

    [HttpPost("condiciones-pago")]
    public async Task<IActionResult> CreateCondicionPago([FromBody] CondicionPagoRequest req)
    {
        var c = CondicionPago.Crear(req.Nombre, req.DiasPlazo);
        _ctx.CondicionesPago.Add(c);
        await _ctx.SaveChangesAsync();
        return Ok(new { c.Id });
    }

    [HttpPut("condiciones-pago/{id}")]
    public async Task<IActionResult> UpdateCondicionPago(Guid id, [FromBody] CondicionPagoRequest req)
    {
        var c = await _ctx.CondicionesPago.FindAsync(id);
        if (c == null) return NotFound();
        c.Actualizar(req.Nombre, req.DiasPlazo);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("condiciones-pago/{id}")]
    public async Task<IActionResult> DeleteCondicionPago(Guid id)
    {
        var c = await _ctx.CondicionesPago.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(false);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("condiciones-pago/{id}/reactivar")]
    public async Task<IActionResult> ReactivarCondicionPago(Guid id)
    {
        var c = await _ctx.CondicionesPago.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(true);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    // ─── OCUPACIONES ──────────────────────────────────────────────────────
    [HttpGet("ocupaciones")]
    public async Task<IActionResult> GetOcupaciones([FromQuery] bool soloActivas = true)
    {
        var q = _ctx.Ocupaciones.AsQueryable();
        if (soloActivas) q = q.Where(x => x.Activa);
        var res = await q.Select(x => new { x.Id, x.Nombre, x.Activa }).ToListAsync();
        return Ok(res);
    }

    [HttpPost("ocupaciones")]
    public async Task<IActionResult> CreateOcupacion([FromBody] OcupacionRequest req)
    {
        var c = Ocupacion.Crear(req.Nombre);
        _ctx.Ocupaciones.Add(c);
        await _ctx.SaveChangesAsync();
        return Ok(new { c.Id });
    }

    [HttpPut("ocupaciones/{id}")]
    public async Task<IActionResult> UpdateOcupacion(Guid id, [FromBody] OcupacionRequest req)
    {
        var c = await _ctx.Ocupaciones.FindAsync(id);
        if (c == null) return NotFound();
        c.Actualizar(req.Nombre);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("ocupaciones/{id}")]
    public async Task<IActionResult> DeleteOcupacion(Guid id)
    {
        var c = await _ctx.Ocupaciones.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(false);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("ocupaciones/{id}/reactivar")]
    public async Task<IActionResult> ReactivarOcupacion(Guid id)
    {
        var c = await _ctx.Ocupaciones.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(true);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    // ─── SECTORES ─────────────────────────────────────────────────────────
    [HttpGet("sectores")]
    public async Task<IActionResult> GetSectores([FromQuery] bool soloActivas = true)
    {
        var q = _ctx.Sectores.AsQueryable();
        if (soloActivas) q = q.Where(x => x.Activa);
        var res = await q.Select(x => new { x.Id, x.Nombre, x.Activa }).ToListAsync();
        return Ok(res);
    }

    [HttpPost("sectores")]
    public async Task<IActionResult> CreateSector([FromBody] SectorRequest req)
    {
        var c = Sector.Crear(req.Nombre);
        _ctx.Sectores.Add(c);
        await _ctx.SaveChangesAsync();
        return Ok(new { c.Id });
    }

    [HttpPut("sectores/{id}")]
    public async Task<IActionResult> UpdateSector(Guid id, [FromBody] SectorRequest req)
    {
        var c = await _ctx.Sectores.FindAsync(id);
        if (c == null) return NotFound();
        c.Actualizar(req.Nombre);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("sectores/{id}")]
    public async Task<IActionResult> DeleteSector(Guid id)
    {
        var c = await _ctx.Sectores.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(false);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("sectores/{id}/reactivar")]
    public async Task<IActionResult> ReactivarSector(Guid id)
    {
        var c = await _ctx.Sectores.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(true);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    // ─── TIPOS CLIENTE ───────────────────────────────────────────────────
    [HttpGet("tipos-cliente")]
    public async Task<IActionResult> GetTiposCliente([FromQuery] bool soloActivas = true)
    {
        var q = _ctx.TiposCliente.AsQueryable();
        if (soloActivas) q = q.Where(x => x.Activa);
        var res = await q.Select(x => new { x.Id, x.Nombre, x.Activa }).ToListAsync();
        return Ok(res);
    }

    [HttpPost("tipos-cliente")]
    public async Task<IActionResult> CreateTipoCliente([FromBody] TipoClienteRequest req)
    {
        var c = TipoCliente.Crear(req.Nombre);
        _ctx.TiposCliente.Add(c);
        await _ctx.SaveChangesAsync();
        return Ok(new { c.Id });
    }

    [HttpPut("tipos-cliente/{id}")]
    public async Task<IActionResult> UpdateTipoCliente(Guid id, [FromBody] TipoClienteRequest req)
    {
        var c = await _ctx.TiposCliente.FindAsync(id);
        if (c == null) return NotFound();
        c.Actualizar(req.Nombre);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("tipos-cliente/{id}")]
    public async Task<IActionResult> DeleteTipoCliente(Guid id)
    {
        var c = await _ctx.TiposCliente.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(false);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("tipos-cliente/{id}/reactivar")]
    public async Task<IActionResult> ReactivarTipoCliente(Guid id)
    {
        var c = await _ctx.TiposCliente.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(true);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    // ─── FORMAS COBRO ───────────────────────────────────────────────────
    [HttpGet("formas-cobro")]
    public async Task<IActionResult> GetFormasCobro([FromQuery] bool soloActivas = true)
    {
        var q = _ctx.FormasCobro.AsQueryable();
        if (soloActivas) q = q.Where(x => x.Activa);
        var res = await q.Select(x => new { x.Id, x.Nombre, x.Activa }).ToListAsync();
        return Ok(res);
    }

    [HttpPost("formas-cobro")]
    public async Task<IActionResult> CreateFormaCobro([FromBody] FormaCobroRequest req)
    {
        var c = FormaCobro.Crear(req.Nombre);
        _ctx.FormasCobro.Add(c);
        await _ctx.SaveChangesAsync();
        return Ok(new { c.Id });
    }

    [HttpPut("formas-cobro/{id}")]
    public async Task<IActionResult> UpdateFormaCobro(Guid id, [FromBody] FormaCobroRequest req)
    {
        var c = await _ctx.FormasCobro.FindAsync(id);
        if (c == null) return NotFound();
        c.Actualizar(req.Nombre);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("formas-cobro/{id}")]
    public async Task<IActionResult> DeleteFormaCobro(Guid id)
    {
        var c = await _ctx.FormasCobro.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(false);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("formas-cobro/{id}/reactivar")]
    public async Task<IActionResult> ReactivarFormaCobro(Guid id)
    {
        var c = await _ctx.FormasCobro.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(true);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    // ─── UNIDADES MEDIDA ─────────────────────────────────────────────────
    [HttpGet("unidades-medida")]
    public async Task<IActionResult> GetUnidadesMedida([FromQuery] bool soloActivas = true)
    {
        var q = _ctx.UnidadesMedida.AsQueryable();
        if (soloActivas) q = q.Where(x => x.Activa);
        var res = await q.Select(x => new { x.Id, x.Nombre, x.Simbolo, x.Activa }).ToListAsync();
        return Ok(res);
    }

    [HttpPost("unidades-medida")]
    public async Task<IActionResult> CreateUnidadMedida([FromBody] UnidadMedidaRequest req)
    {
        var c = UnidadMedida.Crear(req.Nombre, req.Simbolo);
        _ctx.UnidadesMedida.Add(c);
        await _ctx.SaveChangesAsync();
        return Ok(new { c.Id });
    }

    [HttpPut("unidades-medida/{id}")]
    public async Task<IActionResult> UpdateUnidadMedida(Guid id, [FromBody] UnidadMedidaRequest req)
    {
        var c = await _ctx.UnidadesMedida.FindAsync(id);
        if (c == null) return NotFound();
        c.Actualizar(req.Nombre, req.Simbolo);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("unidades-medida/{id}")]
    public async Task<IActionResult> DeleteUnidadMedida(Guid id)
    {
        var c = await _ctx.UnidadesMedida.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(false);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("unidades-medida/{id}/reactivar")]
    public async Task<IActionResult> ReactivarUnidadMedida(Guid id)
    {
        var c = await _ctx.UnidadesMedida.FindAsync(id);
        if (c == null) return NotFound();
        c.SetEstado(true);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }
}

public record CondicionIvaRequest(string Nombre);
public record CondicionPagoRequest(string Nombre, int DiasPlazo);
public record OcupacionRequest(string Nombre);
public record SectorRequest(string Nombre);
public record TipoClienteRequest(string Nombre);
public record FormaCobroRequest(string Nombre);
public record UnidadMedidaRequest(string Nombre, string Simbolo);
