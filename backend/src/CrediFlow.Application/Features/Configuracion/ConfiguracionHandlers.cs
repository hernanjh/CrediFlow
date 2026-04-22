using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Configuracion;

public record GetConfiguracionQuery() : IRequest<Result<ConfiguracionGlobalDto>>;

public class GetConfiguracionHandler : IRequestHandler<GetConfiguracionQuery, Result<ConfiguracionGlobalDto>>
{
    private readonly IUnitOfWork _uow;
    public GetConfiguracionHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<ConfiguracionGlobalDto>> Handle(GetConfiguracionQuery request, CancellationToken ct)
    {
        var config = await _uow.Configuracion.GetAsync(ct);
        if (config == null) 
        {
            config = new ConfiguracionGlobal();
            await _uow.SaveChangesAsync(ct);
        }

        return Result<ConfiguracionGlobalDto>.Success(new ConfiguracionGlobalDto(
            config.NombreEmpresa, config.CUIT, config.RazonSocial, config.Direccion,
            config.Telefono, config.EmailContacto, config.TasaInteresDefecto,
            config.TasaMoraDefecto, config.MonedaSimbolo, config.LogoBase64,
            new List<ListaPrecioDto>()
        ));
    }
}

public record UpdateConfiguracionCommand(
    string NombreEmpresa, string? Cuit, string? RazonSocial, string? Direccion,
    string? Telefono, string? EmailContacto, decimal TasaInteresDefecto,
    decimal TasaMoraDefecto, string MonedaSimbolo, string? LogoBase64) : IRequest<Result>;

public class UpdateConfiguracionHandler : IRequestHandler<UpdateConfiguracionCommand, Result>
{
    private readonly IUnitOfWork _uow;
    public UpdateConfiguracionHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(UpdateConfiguracionCommand req, CancellationToken ct)
    {
        var config = await _uow.Configuracion.GetAsync(ct);
        if (config == null) config = new ConfiguracionGlobal();

        config.NombreEmpresa = req.NombreEmpresa;
        config.CUIT = req.Cuit;
        config.RazonSocial = req.RazonSocial;
        config.Direccion = req.Direccion;
        config.Telefono = req.Telefono;
        config.EmailContacto = req.EmailContacto;
        config.TasaInteresDefecto = req.TasaInteresDefecto;
        config.TasaMoraDefecto = req.TasaMoraDefecto;
        config.MonedaSimbolo = req.MonedaSimbolo;
        config.LogoBase64 = req.LogoBase64;

        _uow.Configuracion.Update(config);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
