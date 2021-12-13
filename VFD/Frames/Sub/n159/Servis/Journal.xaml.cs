using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using iTextSharp.text.pdf;
using Monokot.Hmi.Core.Framework.Runtime;

namespace VFD.Frames.Sub.n159.Servis
{
    /// <summary>
    /// Interaction logic for Journal.xaml
    /// </summary>
    public partial class Journal : Window
    {
        public const string FONT = "c:/windows/fonts/arialbd.ttf";

        public Journal(ServisoJurnalTime time)
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            var uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            var tmp = Path.GetDirectoryName(uri.LocalPath);

            try
            {
                if (time == ServisoJurnalTime.Day)
                {
                    var location = tmp + @"\data\journal\day_journal_template.pdf";
                    var pdf = new PdfReader(location);
                    using (var fw = new FileStream(location.Replace("_template", ""), FileMode.Create))
                    {
                        var stamper = new PdfStamper(pdf, fw);
                        var f = stamper.AcroFields;
                        f.GenerateAppearances = true;
                        var date = DateTime.Now;

                        var arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
                        var arialBaseFont = BaseFont.CreateFont(arialFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        f.AddSubstitutionFont(arialBaseFont);

                        f.SetField("num_day", date.Day.ToString());
                        f.SetField("month_year", date.ToString("MMMM yyyy"));

                        var turn1Start = new DateTime(date.Year, date.Month, date.Day, 8, 0, 0);
                        var turn1End = new DateTime(date.Year, date.Month, date.Day, 20, 0, 0);

                        try
                        {
                            if (turn1Start <= date && date <= turn1End)
                            {
                                f.SetField("count_1", RuntimeManager.Manager.Tags.Node["chtenie.reduktor"].Items["kolichestvo_oborotov"].Value.ToString());
                                f.SetField("hours_1", RuntimeManager.Manager.Expressions.Node["servis.narabotka"].Items[16].Value.ToString());
                                f.SetField("torque_1", RuntimeManager.Manager.Tags.Node["chtenie.reduktor"].Items["srednii_moment"].Value.ToString());
                            }
                            else
                            {
                                f.SetField("count_2", RuntimeManager.Manager.Tags.Node["chtenie.reduktor"].Items["kolichestvo_oborotov"].Value.ToString());
                                f.SetField("hours_2", RuntimeManager.Manager.Expressions.Node["servis.narabotka"].Items[16].Value.ToString());
                                f.SetField("torque_2", RuntimeManager.Manager.Tags.Node["chtenie.reduktor"].Items["srednii_moment"].Value.ToString());
                            }
                        }

                        catch (Exception ex)
                        {
                            // ignored
                        }

                        stamper.FormFlattening = true;
                        stamper.Close();
                    }
                    pdf.Close();
                    Pdf_j.LoadFile(location.Replace("_template", ""));
                }

                if (time == ServisoJurnalTime.Week)
                {
                    var location = tmp + @"\data\journal\week_journal_template.pdf";
                    Pdf_j.LoadFile(location);
                }

                if (time == ServisoJurnalTime.Month)
                {
                    var location = tmp + @"\data\journal\month_journal_template.pdf";
                    Pdf_j.LoadFile(location);
                }

            }
            catch (Exception)
            {
                // ignored
            }
        }


        public enum ServisoJurnalTime
        {
            Day,
            Week,
            Month
        }
    }
}
