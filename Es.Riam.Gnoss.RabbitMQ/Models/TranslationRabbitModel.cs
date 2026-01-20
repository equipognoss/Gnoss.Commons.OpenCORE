using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.RabbitMQ.Models
{
	public class TranslationRabbitModel
	{
		public Guid TranslationID { get; set; }
		public Guid ResourceID { get; set; }
		public DateTimeOffset PublishDate { get; set; }
		public string OriginalLanguage { get; set; }
		public List<string> TargetLanguages { get; set; }
		public Guid UserID { get; set; }
	}
}
