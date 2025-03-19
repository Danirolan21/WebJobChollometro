using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebJobChollometro.Data;
using WebJobChollometro.Models;

namespace WebJobChollometro.Repositories
{
    #region CREATE TABLE
    //create table CHOLLOS
    //(IDCHOLLO int PRIMARY KEY,
    //TITULO NVARCHAR(MAX),
    //LINK NVARCHAR(MAX),
    //DESCRIPCION NVARCHAR(MAX),
    //FECHA datetime)

    #endregion

    public class RepositoryChollometro
    {
        private ChollometroContext context;

        public RepositoryChollometro(ChollometroContext context)
        {
            this.context = context;
        }

        public async Task<int> GetMaxIdCholloAsync()
        {
            if(await this.context.Chollos.CountAsync() == 0)
            {
                return 1;
            }
            else
            {
                return await this.context.Chollos.MaxAsync(x => x.IdChollo) + 1;
            }
        }

        public async Task<List<Chollo>> GetChollosWebAsync()
        {
            string url = "https://www.chollometro.com/rss";
            HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(url);
            request.Accept = @"text/html application/xhtml+xml, *.*";
            request.Host = "www.chollometro.com";
            request.Headers.Add("Accept-language", "es-ES");
            request.Referer = "https://www.chollometro.com";
            request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
            HttpWebResponse response = (HttpWebResponse)
                await request.GetResponseAsync();
            string xmlData = "";
            using (StreamReader reader =
                new StreamReader(response.GetResponseStream()))
            {
                xmlData = await reader.ReadToEndAsync();
            }
            XDocument document = XDocument.Parse(xmlData);
            var consulta = from datos in document.Descendants("item")
                           select datos;
            List<Chollo> chollosWeb = new List<Chollo>();
            int idChollo = await this.GetMaxIdCholloAsync();
            foreach(var tag in consulta)
            {
                Chollo chollo = new Chollo();
                chollo.IdChollo = idChollo;
                chollo.Titulo = tag.Element("title").Value;
                chollo.Descripcion = tag.Element("description").Value;
                chollo.Link = tag.Element("link").Value;
                chollo.Fecha = DateTime.Now;
                idChollo += 1;
                chollosWeb.Add(chollo);
            }
            return chollosWeb;
        }

        //CREAMOS UN METODO PARA AGREGAR 
        public async Task PopulateChollosAzureAsync()
        {
            List<Chollo> chollosWeb = await this.GetChollosWebAsync();
            foreach(Chollo c in chollosWeb)
            {
                //LOS AGREGAMOS AL CONTEXT
                await this.context.Chollos.AddAsync(c);
            }
            await this.context.SaveChangesAsync();
        }
    }
}
