using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyToolkit.Collections;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TablerIcons.Avalonia.Preview.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        foreach (var value in Enum.GetValues<Icons>())
        {
            Icons.Add(new(value, value.ToString()));
        }

        _iconsView = new ObservableCollectionView<IconModel>(Icons);
        _iconsView.Filter = (x) 
            => string.IsNullOrWhiteSpace(SearchText) || x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        //_iconsView.Limit = 1000;
        //_iconsView.Offset = 0;

        TotalPages = GetTotalPages();
        CurrentPage = 1;
    }

    public string Greeting => "Welcome to Avalonia!";

    [ObservableProperty]
    ObservableCollection<IconModel> _icons = new();

    [ObservableProperty]
    ObservableCollectionView<IconModel> _iconsView;

    [ObservableProperty]
    Icons? _selectedIcon;

    [ObservableProperty]
    int _totalPages;

    [ObservableProperty]
    int _currentPage;

    [ObservableProperty]
    float _strokeWidth = 2;

    [ObservableProperty]
    double _width = 90;

    [ObservableProperty]
    double _height = 90;
    partial void OnCurrentPageChanged(int value)
    {
        IconsView.Offset = value * IconsView.Limit;
        GoToFirstPageCommand.NotifyCanExecuteChanged();
        GoToPreviousPageCommand.NotifyCanExecuteChanged();
        GoToNextPageCommand.NotifyCanExecuteChanged();
        GoToLastPageCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    string _searchText;

    CancellationTokenSource? _startSearchCancellationToken;
    partial void OnSearchTextChanged(string value)
    {
        Task.Run(async () =>
        {
            if (_startSearchCancellationToken is not null)
            {
                _startSearchCancellationToken.Cancel();
            }

            try
            {
                _startSearchCancellationToken = new CancellationTokenSource();
                var token = _startSearchCancellationToken.Token;
                await Task.Delay(300, token);

                if (token.IsCancellationRequested)
                    throw new OperationCanceledException();

                IconsView.Refresh();

                Dispatcher.UIThread.Invoke(() =>
                {
                    TotalPages = GetTotalPages();
                    CurrentPage = 1;
                    GoToFirstPageCommand.NotifyCanExecuteChanged();
                    GoToPreviousPageCommand.NotifyCanExecuteChanged();
                    GoToNextPageCommand.NotifyCanExecuteChanged();
                    GoToLastPageCommand.NotifyCanExecuteChanged();
                });
            }
            catch (OperationCanceledException) { }
        });

    }

    private int GetTotalPages()
    {
        var totalCount = Icons.Count(IconsView.Filter);
        var currentCount = IconsView.Count;

        if (currentCount == 0 || totalCount == 0)
        {
            return 1;
        }

        return (totalCount / currentCount);
    }

    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    void GoToFirstPage() => CurrentPage = 1;

    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    void GoToPreviousPage() => CurrentPage--;
    bool CanGoToPreviousPage() => CurrentPage > 1;

    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    void GoToNextPage() => CurrentPage++;

    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    void GoToLastPage() => CurrentPage = TotalPages;
    bool CanGoToNextPage() => CurrentPage < TotalPages;
}

public partial class IconModel : ObservableObject
{

    [ObservableProperty]
    Icons _icon;
    [ObservableProperty]
    string _name;
    
    public IconModel(Icons icon, string name)
    {
        Icon = icon;
        Name = name;
    }
}