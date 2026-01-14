using System.Collections.Generic;

namespace Atelie.Api.Dtos
{
    public class ParsingError
    {
        public int LineNumber { get; set; }
        public string Side { get; set; } = string.Empty; // "left" or "right"
        public string RawText { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ImportResultDto
    {
        public int Imported { get; set; }
        public List<ParsingError> Errors { get; set; } = new List<ParsingError>();
    }
}