using System.Collections.Generic;
using System.Windows;

namespace ClangPowerTools.MVVM.Views
{
  /// <summary>
  /// Interaction logic for EncodingErrorView.xaml
  /// </summary>
  public partial class EncodingErrorView : Window
  {
    public EncodingErrorView(List<string> selectedFiles)
    {
      InitializeComponent();

      var encodingConverter = new EncodingErrorViewModel(selectedFiles);
      encodingConverter.LoadData();
      encodingConverter.CloseAction = () => Close();
      DataContext = encodingConverter;
    }
  }
}
