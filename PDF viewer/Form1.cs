using System;
using System.Windows.Forms;
using PdfiumViewer;

namespace PDF_viewer
{
    public partial class Form1 : Form
    {
        private PdfDocument _pdfDocument; // Поле для хранения PDF-документа
        private PictureBox pictureBox; // Элемент управления для отображения PDF

        public Form1()
        {
            InitializeComponent();
            InitializePdfViewer(); // Инициализация элемента управления для просмотра PDF
        }

        private void InitializePdfViewer()
        {
            // Создаем элемент управления PictureBox для отображения PDF
            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom // Изменяем размер, чтобы заполнить
            };
            this.Controls.Add(pictureBox); // Добавляем PictureBox на форму
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "PDF Files (*.pdf)|*.pdf"; // Фильтр для выбора PDF-файлов
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Загружаем PDF-документ
                    try
                    {
                        _pdfDocument = PdfiumViewer.PdfDocument.Load(openFileDialog.FileName); // Загружаем PDF-документ
                        RenderPage(0); // Рендерим первую страницу
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Не удалось загрузить PDF: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void RenderPage(int pageIndex)
        {
            if (_pdfDocument != null && pageIndex >= 0 && pageIndex < _pdfDocument.PageCount)
            {
                using (var image = _pdfDocument.Render(pageIndex, 300, 300, PdfRenderFlags.Annotations))
                {
                    // Отображаем изображение в элементе управления PictureBox
                    pictureBox.Image = image;
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text; // Получаем текст для поиска
            if (!string.IsNullOrEmpty(searchText) && _pdfDocument != null)
            {
                int pageCount = _pdfDocument.PageCount; // Получаем количество страниц
                bool found = false;

                for (int i = 0; i < pageCount; i++)
                {
                    string pageText = GetPageText(i); // Используем метод для получения текста страницы
                    if (pageText.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        MessageBox.Show($"Найдено на странице {i + 1}");
                        RenderPage(i); // Перейти на страницу, где найден текст
                        break;
                    }
                }

                if (!found)
                {
                    MessageBox.Show("Текст не найден.");
                }
            }
            else
            {
                MessageBox.Show("Введите текст для поиска.");
            }
        }

        // Метод для получения текста страницы
        private string GetPageText(int pageIndex)
        {
            // Получаем текст страницы (учтите, что может потребоваться дополнительная библиотека для извлечения текста)
            var textProvider = _pdfDocument.GetPdfText(pageIndex);
            return textProvider; // Возвращаем текст страницы
        }
    }
}
