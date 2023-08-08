namespace Volleyball_Teams.Views.Templates
{
    /// <summary>
    /// This is a view to display a pair of key value with an icon.
    /// +------+--------------+
    /// |      | Name         |
    /// | Icon | -------------+
    /// |      | Description  |
    /// +------+--------------+
    /// This view is a 2x2 grid. The first column is used to display an icon.
    /// The second column is used to display a key value pair.
    /// </summary>
    public partial class Stars : ContentView
    {
        public static readonly BindableProperty NumStarsProperty = BindableProperty.Create(nameof(NumStars), typeof(string), typeof(Stars), "1", propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (Stars)bindable;
            control.NumStars = newValue as string;
            int stars = int.Parse(control.NumStars);
            control.Star2.IsVisible = false;
            control.Star3.IsVisible = false;
            control.Star4.IsVisible = false;
            control.Star5.IsVisible = false;
            if (stars > 1)
            {
                control.Star2.IsVisible = true;
            }
            if (stars > 2)
            {
                control.Star3.IsVisible = true;
            }
            if (stars > 3)
            {
                control.Star4.IsVisible = true;
            }
            if (stars > 4)
            {
                control.Star5.IsVisible = true;
            }
        });


        public string NumStars
        {
            get => GetValue(NumStarsProperty) as string;
            set => SetValue(NumStarsProperty, value);
        }

        public static readonly BindableProperty StarSizeProperty = BindableProperty.Create(nameof(StarSize), typeof(string), typeof(Stars), "14", propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (Stars)bindable;
            control.StarSize = newValue as string;
            int stars = int.Parse(control.StarSize);
            control.Star1Source.Size = stars;
            control.Star2Source.Size = stars;
            control.Star3Source.Size = stars;
            control.Star4Source.Size = stars;
            control.Star5Source.Size = stars;
        });

        public string StarSize
        {
            get => GetValue(StarSizeProperty) as string;
            set => SetValue(StarSizeProperty, value);
        }

        public Stars()
        {
            InitializeComponent();
        }
    }
}