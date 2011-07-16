using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NhConcurrencyOnAspNetMvc.Models
{
    public class Song
    {
        public virtual int SongId { get; set; }
        public virtual string SongName { get; set; }
        public virtual string AlbumName { get; set; }

        public virtual byte[] Version { get; set; }
    }
}