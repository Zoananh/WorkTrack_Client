using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WorkTrack_WindowsService
{
    class FTP
    {
        string fptserver = "hil.netne.net";
        public bool CheckIfFtpFileExists(string fimename)
        {
            Uri ourUri = new Uri("ftp://hil.netne.net/PCData/" + fimename);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ourUri);
            request.Credentials = new NetworkCredential("a2509114", "qwerty11");
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                //THE FILE EXISTS 


            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (FtpStatusCode.ActionNotTakenFileUnavailable == response.StatusCode)
                {
                    // THE FILE DOES NOT EXIST 
                    return false;
                }
            }
            return true;
        }
        public bool checkFolderFtp(string curDate)
        {
            string ftpHost = "hil.netne.net/";
            string ftpFullPath = "ftp://hil.netne.net/PCData/" + curDate + "/";
            FtpWebRequest ftp;
            try
            {
                ftp = (FtpWebRequest)FtpWebRequest.Create(ftpFullPath);
                ftp.Credentials = new NetworkCredential("a2509114", "qwerty11");
                ftp.KeepAlive = false;
                ftp.UseBinary = true;
                ftp.Proxy = null;
                ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse resp = (FtpWebResponse)ftp.GetResponse();
                resp.Close();
                return true;
            }
            catch
            {
                ftp = (FtpWebRequest)FtpWebRequest.Create(ftpFullPath);
                ftp.Credentials = new NetworkCredential("a2509114", "qwerty11");
                ftp.KeepAlive = false;
                ftp.UseBinary = true;
                ftp.Proxy = null;
                ftp.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse resp = (FtpWebResponse)ftp.GetResponse();
                resp.Close();
                return false;
            }
        }

        public void Upload(string path, string filename)
        {

            FileInfo toUpload = new FileInfo(filename);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://hil.netne.net/PCData/" + path + toUpload.Name);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential("a2509114", "qwerty11");
            Stream ftpStream = request.GetRequestStream();
            FileStream fileStream = File.OpenRead(filename);
            byte[] buffer = new byte[1024]; int bytesRead = 0;
            do
            {
                bytesRead = fileStream.Read(buffer, 0, 1024);
                ftpStream.Write(buffer, 0, bytesRead);
            }

            while (bytesRead != 0);
            fileStream.Close();
            ftpStream.Close();

            /*
            Stream str;
            FileInfo fileInf = new FileInfo(filename);
            string uri = "ftp://hil.netne.net/PCdata/" + path+filename;
            FtpWebRequest reqFTP;
            // Создаем объект FtpWebRequest
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            // Учетная запись
            reqFTP.Credentials = new NetworkCredential("a2509114", "qwerty11");
            reqFTP.KeepAlive = false;
            // Задаем команду на закачку
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // Тип передачи файла
            reqFTP.UseBinary = true;
            // Сообщаем серверу о размере файла
            reqFTP.ContentLength = fileInf.Length;
            // Буффер в 2 кбайт
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            // Файловый поток
            FileStream fs = fileInf.OpenRead();

            str = reqFTP.GetRequestStream();
            // Читаем из потока по 2 кбайт
            contentLen = fs.Read(buff, 0, buffLength);
            // Пока файл не кончится
            while (contentLen != 0)
            {
                str.Write(buff, 0, contentLen);
                contentLen = fs.Read(buff, 0, buffLength);
            }
            // Закрываем потоки
            str.Close();
            fs.Close();*/
        }

        public void Download(string filename)
        {
            try
            {
                //Get FTP web resquest object.
                string uri = "ftp://hil.netne.net/PCdata/" + filename;
                FtpWebRequest request;
                // Создаем объект FtpWebRequest
                request = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                //FtpWebRequest request = FtpWebRequest.Create(new Uri(@"ftp://" + fptserver + @"/" + "Kurs" + @"/" +System.Environment.MachineName+ @"/" fileNameToDownload)) as FtpWebRequest;
                request.UseBinary = true;
                request.KeepAlive = false;
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential("a2509114", "qwerty11");

                FtpWebResponse response = request.GetResponse() as FtpWebResponse;
                Stream responseStream = response.GetResponseStream();
                FileStream outputStream = new FileStream(filename, FileMode.Create);

                int bufferSize = 1024;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = responseStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = responseStream.Read(buffer, 0, bufferSize);
                }
                string statusDescription = response.StatusDescription;
                responseStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error downloading from URL " + "ftp://hil.netne.net/PCdata/" + "/" + filename, ex.Message);
            }
        }
    
}
}
