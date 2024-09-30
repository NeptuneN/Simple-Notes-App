using SQLite;

namespace Simple_Notes_App.Models {
    public class SingleNote {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string BackgroundColor { get; set; }
    }
}