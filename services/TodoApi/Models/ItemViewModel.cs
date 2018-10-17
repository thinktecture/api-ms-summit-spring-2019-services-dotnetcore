namespace TodoApi.Models
{
	public class ItemViewModel
	{
		public int Id { get; set; }
		public int ListId { get; set; }
		public string Text { get; set; }
		public bool Done { get; set; }
	}
}
