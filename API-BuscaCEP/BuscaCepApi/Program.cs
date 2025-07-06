// Inicializa o construtor da aplicação web.
var builder = WebApplication.CreateBuilder(args);

// --- Configuração dos Serviços (Injeção de Dependência) ---

// Registra o IHttpClientFactory para gerenciamento de instâncias HttpClient.
builder.Services.AddHttpClient();

// Registra nosso serviço customizado para ser injetado nos controllers.
builder.Services.AddScoped<IViaCepService, ViaCepService>();

// Define a política de CORS para permitir requisições de outras origens.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Registra os serviços essenciais para o funcionamento dos controllers.
builder.Services.AddControllers();

// Constrói a aplicação com todos os serviços que foram configurados acima.
var app = builder.Build();

// Força o redirecionamento de requisições HTTP para HTTPS.
app.UseHttpsRedirection();

// Aplica a política de CORS definida anteriormente.
app.UseCors("AllowFrontend");

// Mapeia as rotas definidas nos controllers para os endpoints da API.
app.MapControllers();

// Inicia o servidor e o mantém escutando por requisições.
app.Run();