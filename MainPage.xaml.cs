using Simple_Notes_App.Models;
using Simple_Notes_App.Data;
using System.Collections.ObjectModel;

namespace Simple_Notes_App
{
    public partial class MainPage : ContentPage {
        public ObservableCollection<SingleNote> Notes { get; set; }
        // instead of using Color.DeepSkyBlue, etc, just use hexes
        // colors are: DeepSkyBlue, MediumPurple, IndianRed, Black
        private readonly string[] _colorCodes = { "#00BFFF", "#9370DB", "#CD5C5C", "#000000" };
        private string _lastColorCode;
        private NotesDatabase _database;
        private SingleNote _currentNote;

        public MainPage() {
            InitializeComponent();
            Notes = new ObservableCollection<SingleNote>();
            BindingContext = this;
            _lastColorCode = string.Empty; // remember the color code of the last note
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "notes.db3");
            _database = new NotesDatabase(dbPath);
            LoadNotesAsync();
        }

        private async Task LoadNotesAsync() {
            var notesFromDb = await _database.GetNotesAsync();
            Notes.Clear();

            foreach (var note in notesFromDb) {
                Notes.Add(note);
            }
        }

        private async void OnSaveNoteClicked(object sender, EventArgs e) {
            string title = NoteTitle.Text;
            string content = NoteContent.Text;
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(content)) {
                if (_currentNote == null) {
                    string newColorCode = GetRandomColorCode();
                    SingleNote newNote = new SingleNote {
                        Title = title,
                        Content = content,
                        BackgroundColor = newColorCode
                    };
                    await _database.SaveNoteAsync(newNote);
                    Notes.Add(newNote);
                    _lastColorCode = newColorCode;
                }

                else {
                    _currentNote.Title = title;
                    _currentNote.Content = content;
                    await _database.SaveNoteAsync(_currentNote);
                    await LoadNotesAsync();
                    _currentNote = null;
                }

                NoteTitle.Text = string.Empty;
                NoteContent.Text = string.Empty;
            }

            else {
                await DisplayAlert("Error", "Please fill in both title and content", "OK");
            }
        }

        private void OnEditButtonClicked(object sender, EventArgs e) {
            var button = sender as Button;
            var note = button.BindingContext as SingleNote;
            _currentNote = note;
            NoteTitle.Text = note.Title;
            NoteContent.Text = note.Content;
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e) {
            var button = sender as Button;
            var note = button.BindingContext as SingleNote;
            bool answer = await DisplayAlert("Delete Note", "Are you sure you want to delete this note?", "Yes", "No");
            if (answer) {
                await _database.DeleteNoteAsync(note);
                Notes.Remove(note);
            }
        }

        private string GetRandomColorCode() {
            Random random = new Random();
            string randomColorCode;
            // avoid two notes having the same color in a row
            do {
                randomColorCode = _colorCodes[random.Next(_colorCodes.Length)];
            } while (randomColorCode == _lastColorCode);
            return randomColorCode;
        }
    }
}