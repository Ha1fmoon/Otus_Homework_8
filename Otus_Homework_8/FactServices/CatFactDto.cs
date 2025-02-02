namespace Otus_Homework_8.FactServices;

public record CatFactDto
{
    public string Fact { get; init; }
    public int Length { get; init; }

    public CatFactDto(string Fact, int Length)
    {
        this.Fact = Fact;
        this.Length = Length;
    }
}