namespace CrediFlow.Domain.ValueObjects;

public record Dinero
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "ARS";

    public Dinero(decimal amount, string currency = "ARS")
    {
        if (amount < 0) throw new ArgumentException("El monto no puede ser negativo.");
        Amount = amount;
        Currency = currency;
    }

    public static Dinero Zero => new(0);

    public Dinero Add(Dinero other)
    {
        if (Currency != other.Currency) throw new InvalidOperationException("No se pueden sumar monedas distintas.");
        return new Dinero(Amount + other.Amount, Currency);
    }

    public Dinero Subtract(Dinero other)
    {
        if (Currency != other.Currency) throw new InvalidOperationException("No se pueden restar monedas distintas.");
        return new Dinero(Amount - other.Amount, Currency);
    }

    public Dinero Multiply(decimal factor) => new(Amount * factor, Currency);

    public override string ToString() => $"${Amount:N2} {Currency}";
}

public record TasaInteres
{
    public decimal Porcentaje { get; init; }

    public TasaInteres(decimal porcentaje)
    {
        if (porcentaje < 0 || porcentaje > 1000)
            throw new ArgumentException("La tasa debe estar entre 0% y 1000%.");
        Porcentaje = porcentaje;
    }

    public decimal ComoDecimal => Porcentaje / 100m;
    public decimal DiariaSobre30 => ComoDecimal / 30m;

    public override string ToString() => $"{Porcentaje}%";
}
