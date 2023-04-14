namespace TicTacToe.Maui.Views;

public partial class BoardPage : ContentPage
{
    private bool _checkForXorO = false;

    private readonly int[][] _winningCombinations = new int[][]
    {
        new int[] { 0, 1, 2 },
        new int[] { 3, 4, 5 },
        new int[] { 6, 7, 8 },
        new int[] { 0, 3, 6 },
        new int[] { 1, 4, 7 },
        new int[] { 2, 5, 8 },
        new int[] { 0, 4, 8 },
        new int[] { 2, 4, 6 }
    };

    private List<int> _playerX;
    private List<int> _playerO;

    IDispatcherTimer _timer;
    Stopwatch _stopwatch = new Stopwatch();

    public BoardPage()
    {
        InitializeComponent();
        BindingContext = this;

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(1000);
        _timer.Tick += (s, e) => { TimerLabel.Text = $"{_stopwatch.Elapsed.Seconds.ToString()} s"; };
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        ResetGame();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
    }

    /// <summary>
    /// Start a new game
    /// </summary>
    private void ResetGame()
    {
        _playerX = new List<int>();
        _playerO = new List<int>();

        // reset the board
        int i = 0;
        foreach (var button in Board.Children)
        {
            if (button is Button b)
            {
                
                var column = Grid.GetColumn(b);
                var row = Grid.GetRow(b);
                // one could force the text property of a button to represent a number 0...9
                // but luckily the buttons are in a grid in subsequent order as required
                b.Text = i++.ToString();
                b.TextColor = Color.FromRgb(255, 255, 254);
                b.BackgroundColor = Color.FromRgb(255, 255, 255);
                b.FontSize = 100;
                Debug.WriteLine($"{b.Text} in [{column},{row}]");
                b.IsEnabled = true;
            }
        }

        TimerLabel.Text = "0 s";
    }

    /// <summary>
    /// Check for a winner
    /// </summary>
    /// <returns></returns>
    private async Task<bool> CheckForWin()// create a unit test for this method
    {
        foreach (var combination in _winningCombinations)
        {
            int xCount = 0;
            int oCount = 0;
            foreach (var index in combination)
            {
                if (_playerX.Contains(index))
                {
                    xCount++;
                }

                if (_playerO.Contains(index))
                {
                    oCount++;
                }
            }

            if (xCount == 3)
            {
                await DisplayResult("Winner", "Player X wins!");
                return true;
            }

            if (oCount == 3)
            {
                await DisplayResult("Winner", "Player O wins!");
                return true;
            }
        }

        if (_playerX.Count + _playerO.Count == 9)
        {
            await DisplayResult("Draw", "It's a draw!");
            return true;
        }

        return false;
    }

    private async Task DisplayResult(string title, string message)
    {
        if (_stopwatch.IsRunning)
        {
            _stopwatch.Stop();
            _timer.Stop();
        }
        await DisplayAlert(title, message, "OK");
    }


    private async void Button_OnClicked(object sender, EventArgs e)
    {
        if (!_stopwatch.IsRunning)
        {
            _stopwatch.Restart();
            _timer.Start();
        }

        Button button = (Button)sender;
        Debug.WriteLine(button.Text);
        int n = int.Parse(button.Text);
        if (!_checkForXorO)
        {
            button.Text = "X";
            button.TextColor = Color.FromRgb(0,0,0);
            _checkForXorO = true;
            _playerX.Add(n);
        }
        else
        {
            button.Text = "O";
            button.TextColor = Color.FromRgb(255,0,0);
            _checkForXorO = false;
            _playerO.Add(n);
        }

        button.IsEnabled = false;
        if (await CheckForWin())
        {
            ResetGame();
        }
    }
}