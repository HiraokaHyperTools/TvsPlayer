using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VideoConv4Win.DataTypes
{
    public class FFmpegVideo
    {
        [XmlAttribute] public string? Display { get; set; }
        [XmlAttribute] public string? Description { get; set; }
        [XmlAttribute] public string? Options { get; set; }
        [XmlAttribute] public string? FileExtension { get; set; }
    }
}
