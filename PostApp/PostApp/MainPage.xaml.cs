using System.Collections.ObjectModel;
using System.Text.Json;

namespace PostApp
{
    public partial class MainPage : ContentPage
    {
        private readonly string apiUrl = "https://jsonplaceholder.typicode.com/posts";
        private readonly HttpClient _httpClient;
        public ObservableCollection<Post> Posts { get; set; }
        public string SearchQuery { get; set; }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            _httpClient = new HttpClient();
            Posts = new ObservableCollection<Post>();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var posts = JsonSerializer.Deserialize<Post[]>(json);

                    Posts.Clear();
                    foreach (var post in posts)
                    {
                        Posts.Add(post);
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Error al cargar los datos.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Excepción", ex.Message, "OK");
            }
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            SearchQuery = e.NewTextValue;
            await FilterData();
        }

        private async Task FilterData()
        {
            try
            {
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var posts = JsonSerializer.Deserialize<Post[]>(json);

                    Posts.Clear();
                    foreach (var post in posts)
                    {
                        if (string.IsNullOrEmpty(SearchQuery) || post.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                        {
                            Posts.Add(post);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Excepción", ex.Message, "OK");
            }
        }

        private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                var selectedPost = e.CurrentSelection[0] as Post;
                if (selectedPost != null)
                {
                    await Navigation.PushAsync(new PostDetailPage(selectedPost));
                }
            }
        }
    }

    public partial class PostDetailPage : ContentPage
    {
        public PostDetailPage(Post post)
        {
            Title = post.Title;
            Content = new StackLayout
            {
                Padding = new Thickness(10),
                Children =
                {
                    new Label { Text = post.Title, FontSize = 20, FontAttributes = FontAttributes.Bold },
                    new Label { Text = post.Body, FontSize = 16 }
                }
            };
        }
    }
}