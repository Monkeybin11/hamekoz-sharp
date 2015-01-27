﻿//
//  Afip.cs
//
//  Author:
//       Claudio Rodrigo Pereyra Diaz <claudiorodrigo@pereyradiaz.com.ar>
//
//  Copyright (c) 2015 Hamekoz
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
using System.Net;
using System.IO;
using Hamekoz.Data;

namespace Hamekoz.Argentina.Afip
{
	public class Afip
	{
		public Afip ()
		{
		}

		public static void DescargarPadron(bool conDenominacion){
			string urlBase = "http://www.afip.gob.ar/genericos/cInscripcion/archivos";
			string archivo = string.Format ("{0}apellidoNombreDenominacion", conDenominacion ? "" : "SIN");
			string url = string.Format("{0}/{1}.zip", urlBase, archivo);
			using (WebClient webClient = new WebClient ()) {
				webClient.DownloadFile (url, string.Format("AFIP-PadronDeCondicionTributaria-{0}-{1:yyyyMMdd}.zip", archivo, DateTime.Now));
			}
		}

		public static void ImportarPadronUnificado(string archivo, bool denominacion){
			FileStream stream = new FileStream(archivo , FileMode.Open, FileAccess.Read);
			StreamReader reader = new StreamReader(stream);
			DB dbafip = new DB () {
				ConnectionName = "Hamekoz.Argentina.Afip"
			};
			while (!reader.EndOfStream)
			{
				string  linea = reader.ReadLine();
				try {
					RegistroPadron registro = new RegistroPadron (linea, denominacion);
					//HACK cambiar llamda a SP por consulta de texto con posibilidad de agregar denominacion
					dbafip.SP("padronTmpActualizar"
						, "cuit", registro.CUIT
						, "impGanancias", registro.ImpuestoGanancias
						, "impiva", registro.ImpuestoIVA
						, "monotributo", registro.Monotributo
						, "integrantesoc", registro.IntegranteSociedad
						, "empleador", registro.Empleador
						, "actividadmonotributo", registro.ActividadMonotributo
					);
				} catch (Exception ex) {
					Console.WriteLine ("Error en importacion:\n\tRegistro: {0}\n\tError: {1}", linea, ex.Message);
				}
			}
			reader.Close();
		}
	}
}

