// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using JsonHelper.Extensions;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JsonHelper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Brush ButtonBackgroundGray = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDDDDDD"));
    private Timer _copyBackgroundResetTimer;
    private int _rawLength;

    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Pretty print JSON.
    /// Use text from clipboard if input box is empty.
    /// </summary>
    private void Prettify_Click(object sender, RoutedEventArgs e)
    {
        string result;
        _rawLength = 0;

        try
        {
            PasteIfEmpty();

            result = Prettify(InputBox.Text);
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        OutputBox.Text = result;
    }

    /// <summary>
    /// Minify JSON.
    /// Use text from clipboard if input box is empty.
    /// </summary>
    private void Minify_Click(object sender, RoutedEventArgs e)
    {
        string result;
        _rawLength = 0;

        try
        {
            PasteIfEmpty();

            result = Minify(InputBox.Text);
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        OutputBox.Text = result;
    }

    /// <summary>
    /// Minify and JavaScript-encode JSON.
    /// Use text from clipboard if input box is empty.
    /// </summary>
    private void JSEncode_Click(object sender, RoutedEventArgs e)
    {
        string result;
        _rawLength = 0;

        try
        {
            PasteIfEmpty();

            string minified = Minify(InputBox.Text);

            // JS encode.
            result = JsonEncodedText.Encode(minified,
                JavaScriptEncoder.UnsafeRelaxedJsonEscaping).ToString();
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        OutputBox.Text = result;
    }

    /// <summary>
    /// JavaScript-decode and prettify JSON.
    /// Use text from clipboard if input box is empty.
    /// </summary>
    private void JSDecode_Click(object sender, RoutedEventArgs e)
    {
        string result;
        _rawLength = 0;

        try
        {
            PasteIfEmpty();

            result = Prettify(JavaScriptStringDecode(InputBox.Text));
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        OutputBox.Text = result;
    }

    /// <summary>
    /// Minify and Base64-encode JSON.
    /// Use text from clipboard if input box is empty.
    /// </summary>
    private void Base64Encode_Click(object sender, RoutedEventArgs e)
    {
        string result;
        _rawLength = 0;

        try
        {
            PasteIfEmpty();

            string minified = Minify(InputBox.Text);
            _rawLength = minified.Length;

            result = Convert.ToBase64String(Encoding.UTF8.GetBytes(minified));
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        OutputBox.Text = result;
    }

    /// <summary>
    /// Base64-decode and prettify JSON.
    /// Use text from clipboard if input box is empty.
    /// </summary>
    private void Base64Decode_Click(object sender, RoutedEventArgs e)
    {
        string result;
        _rawLength = 0;

        try
        {
            PasteIfEmpty();

            result = Prettify(Encoding.UTF8.GetString(Convert.FromBase64String(InputBox.Text)));
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        OutputBox.Text = result;
    }

    /// <summary>
    /// Minify, GZIP-compress and Base64-encode JSON.
    /// Use text from clipboard if input box is empty.
    /// </summary>
    private void GzipCompress_Click(object sender, RoutedEventArgs e)
    {
        string result;
        _rawLength = 0;

        try
        {
            PasteIfEmpty();

            string minified = Minify(InputBox.Text);

            byte[] inputCompressed = Encoding.UTF8.GetBytes(minified).Zip();
            _rawLength = inputCompressed.Length;

            result = Convert.ToBase64String(inputCompressed);
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        OutputBox.Text = result;
    }

    /// <summary>
    /// Base64-decode, GZIP-decompress and prettify JSON.
    /// Use text from clipboard if input box is empty.
    /// </summary>
    private void GzipDecompress_Click(object sender, RoutedEventArgs e)
    {
        string result;
        _rawLength = 0;

        try
        {
            PasteIfEmpty();

            byte[] inputDecompressed = Convert.FromBase64String(InputBox.Text).Unzip();

            result = Prettify(Encoding.UTF8.GetString(inputDecompressed));

        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        OutputBox.Text = result;
    }

    // <summary>
    /// Swap content of input and output boxes.
    /// </summary>
    private void Swap_Click(object sender, RoutedEventArgs e)
    {
        _rawLength = 0;

        string input = InputBox.Text;
        InputBox.Text = OutputBox.Text;
        OutputBox.Text = input;
    }

    private static string Prettify(string input)
    {
        string cleaned = CleanInput(input);
        var deserialized = JsonSerializer.Deserialize<object>(cleaned);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,

            // Both options currently don't work (as of .NET 6) when serializing/deserializing object.
            //PropertyNamingPolicy = UseCamelCase.IsChecked ?? true
            //    ? JsonNamingPolicy.CamelCase
            //    : null,
            //DefaultIgnoreCondition = IgnoreNullValues.IsChecked ?? true
            //    ? JsonIgnoreCondition.WhenWritingNull
            //    : JsonIgnoreCondition.Never,
        };

        return JsonSerializer.Serialize(deserialized, options);
    }

    private string Minify(string input)
    {
        string cleaned = CleanInput(input);
        var deserialized = JsonSerializer.Deserialize<object>(cleaned);

        var options = new JsonSerializerOptions
        {
            WriteIndented = false,

            // Both options currently don't work (as of .NET 6) when serializing/deserializing object.
            //PropertyNamingPolicy = UseCamelCase.IsChecked ?? true
            //    ? JsonNamingPolicy.CamelCase
            //    : null,
            //DefaultIgnoreCondition = IgnoreNullValues.IsChecked ?? true
            //    ? JsonIgnoreCondition.WhenWritingNull
            //    : JsonIgnoreCondition.Never,
        };

        return JsonSerializer.Serialize(deserialized, options);
    }

    public static string JavaScriptStringDecode(string input)
    {
        // Replace some chars.
        var decoded = input.Replace(@"\'", "'")
                           .Replace(@"\""", @"""")
                           .Replace(@"\/", "/")
                           .Replace(@"\\", @"\")
                           .Replace(@"\t", "\t")
                           .Replace(@"\n", "\n");

        // Replace unicode escaped text.
        var regex = new Regex(@"\\[uU]([0-9A-F]{4})");

        decoded = regex.Replace(
                decoded,
                match => ((char)int.Parse(match.Value[2..], NumberStyles.HexNumber)
            ).ToString(CultureInfo.InvariantCulture));

        return decoded;
    }

    private void PasteIfEmpty()
    {
        if (InputBox.Text.Length == 0)
        {
            InputBox.Text = Clipboard.GetText();
        }
    }

    private static string CleanInput(string input)
    {
        // Remove invalid characters from Teams UTF-8 E2808B.
        string cleaned = Regex.Replace(input, @"\u200B", "", RegexOptions.None);

        return cleaned;
    }

    private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        InputLabel.Text = $"Input ({InputBox.Text.Length:N0} bytes)";
    }

    private void OutputBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_rawLength > 0)
        {
            OutputLabel.Text = $"Output ({OutputBox.Text.Length:N0} bytes | {_rawLength} bytes without Base64 encoding)";
        }
        else
        {
            OutputLabel.Text = $"Output ({OutputBox.Text.Length:N0} bytes)";
        }
    }

    private void InputFromClipboard_Click(object sender, RoutedEventArgs e)
    {
        InputBox.Text = Clipboard.GetText();
    }

    private void OutputToClipboard_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(OutputBox.Text);

        AnimateButtonPress(sender as Button);
    }

    /// <summary>
    /// Highlight button for 1 s, then restore background color.
    /// </summary>
    private void AnimateButtonPress(Button button)
    {
        // Highlight button.
        button.Background = new SolidColorBrush(Colors.LightGreen);

        // Restore background color after a while.
        _copyBackgroundResetTimer = new Timer(async state =>
        {
            await Dispatcher.InvokeAsync(() =>
            {
                button.Background = ButtonBackgroundGray;
            });
        }, state: null, dueTime: 1000, period: 0); // ms.
    }

    private void InputBox_PreviewDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }

    private async void InputBox_PreviewDrop(object sender, DragEventArgs e)
    {
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

        if (files != null && files.Length > 0)
        {
            try
            {
                // Read first file.
                InputBox.Text = await File.ReadAllTextAsync(files[0]);
            }
            catch (Exception ex)
            {
                InputBox.Text = ex.Message;
            }
        }
    }
}
