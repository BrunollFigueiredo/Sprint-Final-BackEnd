namespace BugTracker.DTOs;

public class DashboardDTO
{
    public int TotalBugs { get; set; }
    public int BugsAbertos { get; set; }
    public int BugsEmAndamento { get; set; }
    public int BugsResolvidos { get; set; }
    public int BugsFechados { get; set; }
    public int BugsCriticos { get; set; }
    public int BugsBloqueiamLancamento { get; set; }
    public int TotalProjetos { get; set; }
    public int TotalUsuarios { get; set; }
    public IEnumerable<BugsPorProjetoDTO> BugsPorProjeto { get; set; } = [];
    public IEnumerable<BugsPorSeveridadeDTO> BugsPorSeveridade { get; set; } = [];
    public IEnumerable<BugsPorPlataformaDTO> BugsPorPlataforma { get; set; } = [];
    public IEnumerable<BugsPorTipoDTO> BugsPorTipo { get; set; } = [];
    public IEnumerable<BugBloqueiadorDTO> BugsBloqueiadores { get; set; } = [];
}

public class BugsPorProjetoDTO
{
    public string Nome { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Abertos { get; set; }
}

public class BugsPorSeveridadeDTO
{
    public string Severidade { get; set; } = string.Empty;
    public int Total { get; set; }
}

public class BugsPorPlataformaDTO
{
    public string Plataforma { get; set; } = string.Empty;
    public int Total { get; set; }
}

public class BugsPorTipoDTO
{
    public string Tipo { get; set; } = string.Empty;
    public int Total { get; set; }
}

public class BugBloqueiadorDTO
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string ProjetoNome { get; set; } = string.Empty;
    public string Severidade { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AtribuidoParaNome { get; set; }
}
