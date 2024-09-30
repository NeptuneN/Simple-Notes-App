using SQLite;
using Simple_Notes_App.Models;

namespace Simple_Notes_App.Data {
    public class NotesDatabase {
        private readonly SQLiteAsyncConnection _database;

        public NotesDatabase(string dbPath) {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<SingleNote>().Wait();
        }

        public Task<List<SingleNote>> GetNotesAsync() {
            return _database.Table<SingleNote>().ToListAsync();
        }

        public Task<SingleNote> GetNoteAsync(int id) {
            return _database.Table<SingleNote>().Where(n => n.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveNoteAsync(SingleNote note) {
            if (note.Id != 0) {
                return _database.UpdateAsync(note);
            }

            else {
                return _database.InsertAsync(note);
            }
        }

        public Task<int> DeleteNoteAsync(SingleNote note) {
            return _database.DeleteAsync(note);
        }
    }
}
