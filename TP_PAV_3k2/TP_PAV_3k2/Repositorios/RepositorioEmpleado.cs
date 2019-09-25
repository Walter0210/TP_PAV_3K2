﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP_PAV_3k2.Clases;

namespace TP_PAV_3k2.Repositorios
{
    class RepositorioEmpleado
    {
        private accesoBD _BD;

        public RepositorioEmpleado()
        {
            _BD = new accesoBD();
        }

        public DataTable ObtenerEmpleados()
        {
            //se define una variable local a la función <sqltxt> del tipo <string> donde en el 
            //momento de su creación se le asigan su contenido, que es el comando SELECT  
            //necesario para poder establecer la veracidad del usuario.
            string sqltxt = "SELECT a.legajo, a.nombre, a.apellido, b.nombre, a.nroDoc, a.fechaNacimiento, a.fechaAlta, a.legajoSuperior FROM dbo.Empleado a, TipoDocumento b WHERE a.tipoDoc=b.idTipoDocumento";

            return _BD.consulta(sqltxt);
        }
        public DataTable ObtenerLegajos()
        {
            string sqltxt = "SELECT legajo FROM dbo.Empleado";

            return _BD.consulta(sqltxt);
        }

        public bool Guardar(Empleado empleado)
        {
            string sqltxt="";
            if (empleado.legajoSuperior != -1)
            {
                sqltxt = $"INSERT dbo.Empleado (nombre, apellido, tipoDoc, nroDoc, fechaNacimiento, fechaAlta, legajoSuperior)" +
                    $"VALUES ('{empleado.nombre}', '{empleado.apellido}', (SELECT a.idTipoDocumento FROM TipoDocumento a, Empleado b WHERE a.nombre=b.tipoDoc), '{empleado.nroDoc}', '{empleado.ReturnfechaNacimiento()}', '{empleado.ReturnFechaAlta()}', '{empleado.legajoSuperior}')";
            }
            else
            {
                sqltxt= sqltxt = $"INSERT dbo.Empleado (nombre, apellido, tipoDoc, nroDoc, fechaNacimiento, fechaAlta)" +
                    $"VALUES ('{empleado.nombre}', '{empleado.apellido}', (SELECT a.idTipoDocumento FROM TipoDocumento a, Empleado b WHERE a.nombre='{empleado.tipoDoc}' AND a.idTipoDocumento=b.tipoDoc), '{empleado.nroDoc}', '{empleado.ReturnfechaNacimiento()}', '{empleado.ReturnFechaAlta()}')";
            }
            return _BD.EjecutarSQL(sqltxt);
        }

        public bool Eliminar(string legajo)
        {
            string sqltxt = $"DELETE FROM [dbo].[Empleado] WHERE legajo = {legajo}";

            return _BD.EjecutarSQL(sqltxt);
        }
        public DataTable SoySupervisorActivo(string legajo)
        {
            string sqltxt = $"SELECT * FROM empleado WHERE legajoSuperior='{legajo}";
            return _BD.consulta(sqltxt);
        }
        public Empleado ObtenerEmpleado(string legajo)
        {
            string sqltxt =$"SELECT a.legajo, a.nombre, a.apellido, b.nombre, a.nroDoc, a.fechaNacimiento, a.fechaAlta, a.legajoSuperior FROM dbo.Empleado a, TipoDocumento b WHERE a.tipoDoc = b.idTipoDocumento AND legajo='{legajo}'";
            var tablaTemporal = _BD.consulta(sqltxt);

            if (tablaTemporal.Rows.Count == 0)
                return null;
            var empleado = new Empleado();
            foreach (DataRow fila in tablaTemporal.Rows)
            {
                DateTime fecha1;
                DateTime fecha2;
                if (fila.HasErrors)
                    continue;
                empleado.legajo = int.Parse(fila.ItemArray[0].ToString());
                empleado.nombre = fila.ItemArray[1].ToString();
                empleado.apellido = fila.ItemArray[2].ToString();
                empleado.tipoDoc = fila.ItemArray[3].ToString();
                empleado.nroDoc = int.Parse(fila.ItemArray[4].ToString());
                DateTime.TryParse(fila.ItemArray[5]?.ToString(), out fecha1);
                empleado.fechaNacimiento = fecha1;
                DateTime.TryParse(fila.ItemArray[6]?.ToString(), out fecha2);
                empleado.fechaAlta = fecha2;

            }
            return empleado;
        }

        public bool Actualizar(Empleado empleado)
        {
            string sqltxt = $"UPDATE dbo.Empleado SET nombre = '{empleado.nombre}', " +
                $"apellido = '{empleado.apellido}', tipoDoc= (SELECT a.idTipoDocumento FROM TipoDocumento a, Empleado b WHERE a.nombre=b.tipoDoc), " +
                $"nroDoc = '{empleado.nroDoc}', fechaNacimiento= '{empleado.fechaNacimiento.ToString("yyyy-MM-dd")}', fechaAlta='{empleado.fechaAlta.ToString("yyyy-MM-dd")}', legajoSuperior='{empleado.legajoSuperior}'" +
                $"WHERE legajo={empleado.legajo}";

            return _BD.EjecutarSQL(sqltxt);
        }
    }
}
