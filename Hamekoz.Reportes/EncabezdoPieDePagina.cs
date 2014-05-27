//
//  EncabezdoPieDePagina.cs
//
//  Author:
//       Claudio Rodrigo Pereyra Diaz <claudiorodrigo@pereyradiaz.com.ar>
//
//  Copyright (c) 2010 Hamekoz
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Text;

using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Hamekoz.Reportes
{
	public class EncabezdoPieDePagina : PdfPageEventHelper
	{
		// This is the contentbyte object of the writer
		PdfContentByte cb;

		// we will put the final number of pages in a template
		PdfTemplate template;

		Font fuente = FontFactory.GetFont (FontFactory.HELVETICA_BOLD, 8, BaseColor.LIGHT_GRAY);

		// This keeps track of the creation time
		DateTime PrintTime = DateTime.Now;

		#region Properties
		private string _Title = string.Empty;
		public string Title {
			get { return _Title; }
			set { _Title = value; }
		}

		private string _Footer = string.Empty;
		public string Footer {
			get { return _Footer; }
			set { _Footer = value; }
		}

		private string _HeaderLeft = string.Empty;
		public string HeaderLeft {
			get { return _HeaderLeft; }
			set { _HeaderLeft = value; }
		}

		private string _HeaderRight = string.Empty;
		public string HeaderRight {
			get { return _HeaderRight; }
			set { _HeaderRight = value; }
		}

		private Font _HeaderFont;
		public Font HeaderFont {
			get { return _HeaderFont; }
			set { _HeaderFont = value; }
		}

		private Font _FooterFont;
		public Font FooterFont {
			get { return _FooterFont; }
			set { _FooterFont = value; }
		}
		#endregion

		public override void OnOpenDocument (PdfWriter writer, Document document)
		{
			PrintTime = DateTime.Now;
			cb = writer.DirectContent;
			cb.SetColorStroke (BaseColor.LIGHT_GRAY);
			cb.SetColorFill (BaseColor.LIGHT_GRAY);
			template = cb.CreateTemplate (50, 50);
		}

		public override void OnStartPage (PdfWriter writer, Document document)
		{
			base.OnStartPage (writer, document);

			Rectangle pageSize = document.PageSize;

			cb.SetColorStroke (BaseColor.LIGHT_GRAY);

			PdfPTable HeaderTable = new PdfPTable (3);

			HeaderTable.SetWidthPercentage(new float[] { 20, 60, 20 }, PageSize.A4);

			HeaderTable.DefaultCell.Border = PdfPCell.NO_BORDER;
			HeaderTable.TotalWidth = pageSize.Width - document.LeftMargin - document.RightMargin;

			PdfPCell HeaderLeftCell = new PdfPCell (new Phrase (8, HeaderLeft, fuente));
			HeaderLeftCell.Border = PdfPCell.NO_BORDER;
			HeaderTable.AddCell (HeaderLeftCell);

			PdfPCell HeaderCenterCell = new PdfPCell (new Phrase (8, Title, fuente));
			HeaderCenterCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			HeaderCenterCell.Border = PdfPCell.NO_BORDER;
			HeaderTable.AddCell (HeaderCenterCell);

			PdfPCell HeaderRightCell = new PdfPCell (new Phrase (8, HeaderRight, fuente));
			HeaderRightCell.Border = PdfPCell.NO_BORDER;
			HeaderRightCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
			HeaderTable.AddCell (HeaderRightCell);

			cb.MoveTo (pageSize.GetLeft (document.LeftMargin), pageSize.GetTop (document.TopMargin - 5));
			cb.LineTo (pageSize.GetRight (document.RightMargin), pageSize.GetTop (document.TopMargin - 5));
			cb.Stroke ();

			HeaderTable.WriteSelectedRows (0, -1, pageSize.GetLeft (document.LeftMargin), pageSize.GetTop (document.TopMargin - 20), cb);
		}

		public override void OnEndPage (PdfWriter writer, Document document)
		{
			base.OnEndPage (writer, document);

			int pageN = writer.PageNumber;
			String text = "Página " + pageN + " de ";
			float len = fuente.BaseFont.GetWidthPoint(text, fuente.Size) + 1;

			Rectangle pageSize = document.PageSize;

			cb.AddTemplate (template, pageSize.GetLeft (document.LeftMargin) + len, pageSize.GetBottom (document.BottomMargin - 10));

			PdfPTable FooterTable = new PdfPTable (3);
			FooterTable.DefaultCell.Border = PdfPCell.NO_BORDER;
			FooterTable.TotalWidth = pageSize.Width - document.LeftMargin - document.RightMargin;

			PdfPCell FooterLeftCell = new PdfPCell (new Phrase (8, text, fuente));
			FooterLeftCell.Border = PdfPCell.NO_BORDER;
			FooterTable.AddCell (FooterLeftCell);

			PdfPCell FooterCenterCell = new PdfPCell (new Phrase (8, Footer, fuente));
			FooterCenterCell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
			FooterCenterCell.Border = PdfPCell.NO_BORDER;
			FooterTable.AddCell (FooterCenterCell);

            string texto = String.Format("Generado el {0} {1}", PrintTime.ToShortDateString(), PrintTime.ToShortTimeString());
			//string texto = "";
            PdfPCell FooterRightCell = new PdfPCell (new Phrase (8, texto , fuente));
			FooterRightCell.Border = PdfPCell.NO_BORDER;
			FooterRightCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
			FooterTable.AddCell (FooterRightCell);


			cb.MoveTo (pageSize.GetLeft (document.LeftMargin), pageSize.GetBottom (document.BottomMargin));
			cb.LineTo (pageSize.GetRight (document.RightMargin), pageSize.GetBottom (document.BottomMargin));
			cb.Stroke ();

			FooterTable.WriteSelectedRows (0, -1, pageSize.GetLeft (document.LeftMargin), pageSize.GetBottom (document.BottomMargin), cb);
		}

		public override void OnCloseDocument (PdfWriter writer, Document document)
		{
			base.OnCloseDocument (writer, document);
			template.BeginText ();
			template.SetFontAndSize (fuente.BaseFont, fuente.Size);
			template.SetColorFill (fuente.Color);
			template.SetTextMatrix (0, 0);
			template.ShowText ("" + (writer.PageNumber - 1));
			template.EndText ();
		}
	}
}