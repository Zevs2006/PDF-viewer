using System;
using System.Windows.Forms;
using PdfiumViewer;

namespace PDF_viewer
{
    public partial class Form1 : Form
    {
        private PdfDocument _pdfDocument; // ���� ��� �������� PDF-���������
        private PictureBox pictureBox; // ������� ���������� ��� ����������� PDF

        public Form1()
        {
            InitializeComponent();
            InitializePdfViewer(); // ������������� �������� ���������� ��� ��������� PDF
        }

        private void InitializePdfViewer()
        {
            // ������� ������� ���������� PictureBox ��� ����������� PDF
            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom // �������� ������, ����� ���������
            };
            this.Controls.Add(pictureBox); // ��������� PictureBox �� �����
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "PDF Files (*.pdf)|*.pdf"; // ������ ��� ������ PDF-������
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // ��������� PDF-��������
                    try
                    {
                        _pdfDocument = PdfiumViewer.PdfDocument.Load(openFileDialog.FileName); // ��������� PDF-��������
                        RenderPage(0); // �������� ������ ��������
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"�� ������� ��������� PDF: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    // ���������� ����������� � �������� ���������� PictureBox
                    pictureBox.Image = image;
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text; // �������� ����� ��� ������
            if (!string.IsNullOrEmpty(searchText) && _pdfDocument != null)
            {
                int pageCount = _pdfDocument.PageCount; // �������� ���������� �������
                bool found = false;

                for (int i = 0; i < pageCount; i++)
                {
                    string pageText = GetPageText(i); // ���������� ����� ��� ��������� ������ ��������
                    if (pageText.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        MessageBox.Show($"������� �� �������� {i + 1}");
                        RenderPage(i); // ������� �� ��������, ��� ������ �����
                        break;
                    }
                }

                if (!found)
                {
                    MessageBox.Show("����� �� ������.");
                }
            }
            else
            {
                MessageBox.Show("������� ����� ��� ������.");
            }
        }

        // ����� ��� ��������� ������ ��������
        private string GetPageText(int pageIndex)
        {
            // �������� ����� �������� (������, ��� ����� ������������� �������������� ���������� ��� ���������� ������)
            var textProvider = _pdfDocument.GetPdfText(pageIndex);
            return textProvider; // ���������� ����� ��������
        }
    }
}
