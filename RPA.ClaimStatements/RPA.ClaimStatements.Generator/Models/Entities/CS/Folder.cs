using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("Folders", Schema="CS")]
    public class Folder
    {
        public Guid FolderID { get; set; }

        public string Description { get; set; }

        public string Path { get; set; }

        public string Mask { get; set; }

        public string Host { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public Folder()
        {
            FolderID = Guid.NewGuid();
        }

        public Folder(string description, string path = null, string mask = null):this()
        {
            Description = description;
            Path = path;
            Mask = mask;
        }
    }
}