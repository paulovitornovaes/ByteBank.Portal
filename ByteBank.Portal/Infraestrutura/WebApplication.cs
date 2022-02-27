using ByteBank.Service.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ByteBank.Portal.Infraestrutura
{
    public class WebApplication
    {
        private readonly string[] _prefixos;
        public WebApplication(string[] prefixos)
        {
            _prefixos = prefixos ?? throw new ArgumentNullException(nameof(prefixos));
        }
        public void Iniciar()
        {
            while (true)
            {
                ManipularRequisicao();
            }
        }

        private void ManipularRequisicao()
        {
            var httpListener = new HttpListener();

            foreach (var prefixo in _prefixos)
            {
                httpListener.Prefixes.Add(prefixo);
            }
            httpListener.Start();

            var contexto = httpListener.GetContext();

            var requisicao = contexto.Request;
            var resposta = contexto.Response;

            var path = requisicao.Url.AbsolutePath;

            if (Utilidades.EhArquivo(path))
            {
                var manipulador = new ManipuladorRequisicaoArquivo();
                manipulador.Manipular(resposta, path);
            }
            else if(path == "/Cambio/MXN")
            {
                var controller = new CambioController();
                var paginaConteudo = controller.MXN();

                var bufferArquivo = Encoding.UTF8.GetBytes(paginaConteudo);
                
                resposta.StatusCode = 200;
                resposta.ContentType = "text/html; charset=utf-8;";
                resposta.ContentLength64 = bufferArquivo.Length;
                
                resposta.OutputStream.Write(bufferArquivo, 0, bufferArquivo.Length);
                resposta.OutputStream.Close();

            }

            else if (path == "/Cambio/USD")
            {
                var controller = new CambioController();
                var paginaConteudo = controller.USD();

                var bufferArquivo = Encoding.UTF8.GetBytes(paginaConteudo);

                resposta.StatusCode = 200;
                resposta.ContentType = "text/html; charset=utf-8;";
                resposta.ContentLength64 = bufferArquivo.Length;

                resposta.OutputStream.Write(bufferArquivo, 0, bufferArquivo.Length);
                resposta.OutputStream.Close();

            }
            httpListener.Stop();
        }
    }
}
