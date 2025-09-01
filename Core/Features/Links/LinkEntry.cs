namespace ChatPlus.Core.Features.Links;

public record LinkEntry(string Display, string Url)
{
    public string Tag => $"[l:{Url}]";
}
