﻿using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bvmfscrapper.models
{

    public class ScrappedCompany
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CompanyExtensions));

        public string RazaoSocial { get; set; }
        public string NomePregao { get; set; }
        public SegmentoEnum Segmento { get; set; }
        public int CodigoCVM { get; set; }
        public string CNPJ { get; set; }
        public string[] AtividadePrincipal { get; set; }
        public string[] ClassificacaoSetorial { get; set; }
        public string Site { get; set; }
        public SortedSet<string> CodigosNegociacao { get; set; }
        public DateTime UltimaAtualizacao { get; set; }

        public bool NeedsUpdate { get; set; } = true; // by default needs to be extracted


        public void Save()
        {
            string path = this.GetFileName();

            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Console.WriteLine("Salvando arquivo");
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static ScrappedCompany Load(string filename)
        {

            string filecontent = File.ReadAllText(filename);
            ScrappedCompany deserialized = JsonConvert.DeserializeObject<ScrappedCompany>(filecontent);
            return deserialized;
        }

        public static List<ScrappedCompany> LoadCompaniesFromFiles(string dir)
        {
            log.Info($"Carregando empresas do diretório {dir}");
            List<ScrappedCompany> companies = new List<ScrappedCompany>();

            if (!Directory.Exists(dir))
            {
                log.Fatal($"Diretório {dir} não existe");
                throw new DirectoryNotFoundException();
            }

            var files = Directory.GetFiles(dir, "*.json");
            foreach (var file in files)
            {
                if (file.Contains(".links.json"))
                {
                    // ignore - this is a links file
                    log.Info($"ignorando arquivo {file} por ser arquivo de links");
                    continue;
                }

                log.Info($"Carregando arquivo {file}");
                var company = Load(file);
                if (Options.Instance.Company > 0)
                {
                    if (company.CodigoCVM.ToString().CompareTo(Options.Instance.Company.ToString()) < 0)
                    {
                        continue;
                    }
                }
                companies.Add(company);
            }

            return companies;
        }



    }
}
