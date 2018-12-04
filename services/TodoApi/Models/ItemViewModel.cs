namespace TodoApi.Models
{
    /// <summary>
    /// Represents a Todo list item
    /// </summary>
    public class ItemViewModel
    {
        /// <summary>
        /// Gets or sets the id of the item
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the id of the list this item belongs to
        /// </summary>
        public int ListId { get; set; }
        /// <summary>
        /// Gets or sets the name of this todo item
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Gets or sets whether this item is done or not
        /// </summary>
        public bool Done { get; set; }
    }
}
