using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPGSQLTypeMapIssue.Domain
{
    [Table("poco")]
    public class Poco
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("created_by")]
        public string CreatedBy { get; set; }
    }
}
