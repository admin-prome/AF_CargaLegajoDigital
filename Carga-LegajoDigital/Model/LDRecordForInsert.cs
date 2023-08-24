using LegajoDigitalDemoApp.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegajoDigitalApp.Model
{
    internal class LDRecordForInsert
    {
        public LDRecordForInsert() 
        {
            this.fechaConsulta = GenerarFechaDeConsulta();
            this.apiDavoServiceResponse = new LDServiceResponse();
        }

        private string GenerarFechaDeConsulta()
        {
            DateTime fecha = DateTime.Now;
            return fecha.ToShortDateString();
        }



        internal LDRecordForInsert AdjuntarDatosDeRegistro(LDServiceResponse result, string nif)
        {
            this.NIF = nif;
            this.apiDavoServiceResponse.bp730 = result.bp730;
            this.apiDavoServiceResponse.bp733 = result.bp733;
            this.apiDavoServiceResponse.bp510 = result.bp510;
            this.apiDavoServiceResponse.dni_servicios = result.dni_servicios;
            this.apiDavoServiceResponse.bp935 = result.bp935;
            this.apiDavoServiceResponse.cuil = result.cuil;

            return this;
        }

        public string NIF { get; set; }

        public string fechaConsulta { get; set; }

        public LDServiceResponse apiDavoServiceResponse {get;set;}
    }
}
