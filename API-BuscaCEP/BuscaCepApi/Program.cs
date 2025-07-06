// Inicializa o construtor da aplica��o web.
var builder = WebApplication.CreateBuilder(args);

// --- Configura��o dos Servi�os (Inje��o de Depend�ncia) ---

// Registra o IHttpClientFactory para gerenciamento de inst�ncias HttpClient.
builder.Services.AddHttpClient();

// Registra nosso servi�o customizado para ser injetado nos controllers.
builder.Services.AddScoped<IViaCepService, ViaCepService>();

// Define a pol�tica de CORS para permitir requisi��es de outras origens.
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

// Registra os servi�os essenciais para o funcionamento dos controllers.
builder.Services.AddControllers();

// Constr�i a aplica��o com todos os servi�os que foram configurados acima.
var app = builder.Build();

// For�a o redirecionamento de requisi��es HTTP para HTTPS.
app.UseHttpsRedirection();

// Aplica a pol�tica de CORS definida anteriormente.
app.UseCors("AllowFrontend");

// Mapeia as rotas definidas nos controllers para os endpoints da API.
app.MapControllers();

// Inicia o servidor e o mant�m escutando por requisi��es.
app.Run();