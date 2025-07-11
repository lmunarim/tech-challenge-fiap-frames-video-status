using fiap.Application.Interfaces;
using fiap.Domain.Entities;
using fiap.Domain.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace fiap.Application.UseCases
{
    public class EmailApplication: IEmailApplication
    {
        public readonly ISecretManagerService _secretService;
        public EmailApplication(ISecretManagerService secret)
        {
            _secretService = secret;
        }
        public async Task SendEmailAsync(VideoUpload video)
        {
            try
            {
                var secret = await _secretService.ObterSecret<SecretEmail>("dev/fiap/smtp");

                var smtpClient = new SmtpClient(secret.Smtp)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(secret.User, secret.Pass),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(secret.MailAdress),
                    Subject = "[Status Processamento Video] - FIAP FASE 5 - Notificação Upload Video",
                    Body = $"FIAP FASE 5 - Notificação Upload Video \n Olá {video.Usuario.Nome}, segue o status do processamento do video \n Status de envio - {video.StatusUpload}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(video.Usuario.Email);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }
    }
}




    