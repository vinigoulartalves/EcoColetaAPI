package br.com.ecocoleta;

import com.fasterxml.jackson.databind.*;
import com.fasterxml.jackson.databind.node.ObjectNode;
import io.cucumber.java.*;
import io.cucumber.java.pt.*;
import io.restassured.response.Response;
import io.restassured.specification.RequestSpecification;
import java.math.BigDecimal;
import java.nio.file.*;
import java.time.Instant;
import java.util.*;
import static io.restassured.RestAssured.given;
import static io.restassured.module.jsv.JsonSchemaValidator.matchesJsonSchemaInClasspath;
import static org.junit.jupiter.api.Assertions.*;

/** One instance per scenario. No shared token, fixture or response state. */
public class ApiSteps {
    private static final ObjectMapper JSON = new ObjectMapper();
    private final Map<String, String> variables = new HashMap<>();
    private final List<Map<String, Object>> exchanges = new ArrayList<>();
    private final String baseUrl = setting("baseUrl", "BASE_URL", "http://localhost:8080");
    private final String email = setting("adminEmail", "ADMIN_EMAIL", "admin@ecocoleta.test");
    private final String password = setting("adminPassword", "ADMIN_PASSWORD", "");
    private String token;
    private Response response;
    private Map<String, Object> point;
    private BigDecimal initialWeight;
    private BigDecimal initialPlastic;
    private int initialActive, initialAlerts;
    private String scenarioName;

    private static String setting(String property, String env, String fallback) {
        return System.getProperty(property, System.getenv().getOrDefault(env, fallback));
    }
    @Before
    public void before(Scenario scenario) {
        scenarioName = scenario.getName();
        variables.put("adminEmail", email);
        variables.put("adminPassword", password);
        assertTrue(baseUrl.startsWith("http://localhost:") || baseUrl.startsWith("http://127.0.0.1:")
            || baseUrl.startsWith("http://api:"), "Use somente o ambiente isolado de testes.");
    }
    @After
    public void after(Scenario scenario) throws Exception {
        String evidence = JSON.writerWithDefaultPrettyPrinter().writeValueAsString(exchanges);
        scenario.attach(evidence, "application/json", "HTTP sanitizado - " + scenario.getStatus());
        Path output = Path.of("target/cucumber/http-evidence.jsonl");
        Files.createDirectories(output.getParent());
        Files.writeString(output, JSON.writeValueAsString(Map.of("scenario", scenarioName,
            "status", scenario.getStatus().name(), "timestamp", Instant.now().toString(),
            "exchanges", exchanges)) + "\n", StandardOpenOption.CREATE, StandardOpenOption.APPEND);
    }
    private String resolve(String value) {
        for (var variable : variables.entrySet())
            value = value.replace("${" + variable.getKey() + "}", variable.getValue());
        assertFalse(value.contains("${"), "Variável não resolvida: " + value.replace(password.isEmpty() ? "\0" : password, "***"));
        return value;
    }
    private Object sanitized(String body) {
        if (body == null || body.isBlank()) return "";
        try {
            JsonNode tree = JSON.readTree(body);
            redact(tree);
            return tree;
        } catch (Exception ignored) { return "[conteúdo não JSON omitido]"; }
    }
    private void redact(JsonNode node) {
        if (node.isObject()) {
            var fields = node.fields();
            while (fields.hasNext()) {
                var field = fields.next();
                if (Set.of("token", "senha", "password").contains(field.getKey().toLowerCase(Locale.ROOT)))
                    ((ObjectNode) node).put(field.getKey(), "[REDACTED]");
                else redact(field.getValue());
            }
        } else if (node.isArray()) node.forEach(this::redact);
    }
    private Response call(String method, String path, String body) {
        path = resolve(path);
        String payload = body == null ? null : resolve(body);
        RequestSpecification request = given().baseUri(baseUrl)
            .config(io.restassured.config.RestAssuredConfig.config().httpClient(
                io.restassured.config.HttpClientConfig.httpClientConfig()
                .setParam("http.connection.timeout", 10000).setParam("http.socket.timeout", 15000)))
            .accept("application/json");
        if (token != null) request.header("Authorization", "Bearer " + token);
        if (payload != null) request.contentType("application/json; charset=UTF-8").body(payload);
        response = request.request(method, path);
        Map<String, Object> record = new LinkedHashMap<>();
        record.put("method", method); record.put("path", path);
        record.put("statusCode", response.statusCode());
        record.put("request", sanitized(payload)); record.put("response", sanitized(response.asString()));
        exchanges.add(record);
        return response;
    }
    private void expect(int status) {
        assertEquals(status, response.statusCode(), () -> "Status inesperado; resposta sanitizada: " + sanitized(response.asString()));
    }
    private String toJson(Object value) {
        try { return JSON.writeValueAsString(value); } catch (Exception e) { throw new IllegalArgumentException(e); }
    }
    private void schema(String name) {
        assertTrue(response.contentType().contains("application/json") || response.contentType().contains("application/problem+json"), "Content-Type deve ser JSON ou Problem JSON");
        response.then().body(matchesJsonSchemaInClasspath("schemas/" + name + ".json"));
    }
    @Dado("que estou autenticado como administrador")
    public void admin() {
        assertFalse(password.isBlank(), "Defina ADMIN_PASSWORD para o ambiente de teste.");
        call("POST", "/api/auth/login", toJson(Map.of("email", email, "senha", password)));
        expect(200); schema("login"); token = response.jsonPath().getString("token");
        assertFalse(token.isBlank());
    }
    @Dado("que estou sem autenticação")
    public void anonymous() { token = null; }
    @Dado("que uso um token inválido")
    public void invalidToken() { token = "token.invalido.teste"; }
    @Dado("que existe um ponto de coleta ativo para plástico com capacidade de {int} kg")
    public void createPoint(int capacity) {
        if (token == null) admin();
        point = new LinkedHashMap<>();
        point.put("nome", "BDD " + UUID.randomUUID()); point.put("endereco", "Rua dos Testes, 100");
        point.put("bairro", "Centro BDD"); point.put("cidade", "Palhoca BDD");
        point.put("latitude", -27.64); point.put("longitude", -48.67);
        point.put("tipoResiduoAceito", 1); point.put("capacidadeMaximaKg", capacity);
        call("POST", "/api/pontos-coleta", toJson(point)); expect(201); schema("ponto");
        variables.put("pontoId", response.jsonPath().getString("id"));
        variables.put("pontoNome", point.get("nome").toString());
    }
    @Dado("que o ponto está inativo")
    public void inactive() {
        point.put("ativo", false);
        call("PUT", "/api/pontos-coleta/${pontoId}", toJson(point)); expect(200); schema("ponto");
    }
    @Dado("que foram registrados {word} kg de plástico no ponto")
    public void deposit(String weight) {
        call("POST", "/api/registros-residuos", toJson(Map.of("pontoColetaId", Integer.parseInt(variables.get("pontoId")),
            "tipoResiduo", 1, "pesoKg", new BigDecimal(weight), "origem", "BDD FIAP")));
        expect(201); schema("registro");
    }
    @Dado("que existe um alerta aberto no ponto")
    public void openAlert() {
        createPoint(100); deposit("80");
        call("GET", "/api/alertas-coleta?pontoColetaId=${pontoId}&resolvido=false", null);
        expect(200); schema("alertas"); assertEquals(1, response.jsonPath().getInt("totalItens"));
        variables.put("alertaId", response.jsonPath().getString("itens[0].id"));
    }
    @Dado("que o alerta já foi resolvido")
    public void resolveAlert() {
        call("PATCH", "/api/alertas-coleta/${alertaId}/resolver", null); expect(200); schema("alerta");
    }
    @Dado("que a capacidade do ponto foi alterada para {int} kg")
    public void capacity(int capacity) {
        point.put("capacidadeMaximaKg", capacity); point.put("ativo", true);
        call("PUT", "/api/pontos-coleta/${pontoId}", toJson(point)); expect(200); schema("ponto");
    }
    @Dado("que não há destinação cadastrada para Outros")
    public void availableDestination() {
        // Test database seeds every type. Remove only Outros via the isolated fixture script before a run.
        call("GET", "/api/destinacoes-residuos/9", null); expect(404); schema("erro");
    }
    @Dado("que capturei os totais atuais do relatório")
    public void reportBaseline() {
        call("GET", "/api/relatorios-impacto/resumo", null); expect(200); schema("relatorio");
        initialWeight = decimal("totalResiduosRegistradosKg");
        Object plastic = response.jsonPath().get("residuosPorTipo.Plastico");
        initialPlastic = plastic == null ? BigDecimal.ZERO : new BigDecimal(plastic.toString());
        initialActive = response.jsonPath().getInt("pontosColetaAtivos");
        initialAlerts = response.jsonPath().getInt("alertasAbertos");
    }
    @Quando("envio {string} para {string}")
    public void request(String method, String path) { call(method, path, null); }
    @Quando("envio {string} para {string} com:")
    public void requestBody(String method, String path, String body) { call(method, path, body); }
    @Então("o status HTTP deve ser {int}")
    public void status(int status) { expect(status); }
    @Então("o contrato JSON deve ser {string}")
    public void contract(String name) { schema(name); }
    @Então("o campo {string} deve ser {string}")
    public void textField(String path, String expected) {
        Object actual = response.jsonPath().get(path);
        assertNotNull(actual, "Campo ausente: " + path);
        assertEquals(resolve(expected), actual.toString());
    }
    private BigDecimal decimal(String path) {
        Object actual = response.jsonPath().get(path);
        assertNotNull(actual, "Campo numérico ausente: " + path);
        return new BigDecimal(actual.toString());
    }
    @Então("o campo numérico {string} deve ser {word}")
    public void numberField(String path, String expected) {
        assertEquals(0, decimal(path).compareTo(new BigDecimal(expected)), path);
    }
    @Então("a mensagem deve conter {string}")
    public void message(String expected) { assertTrue(response.jsonPath().getString("mensagem").contains(expected)); }
    @Então("o campo {string} não deve estar vazio")
    public void notEmpty(String path) { assertFalse(response.jsonPath().getString(path).isBlank()); }
    @Então("a resposta deve estar sem corpo")
    public void emptyBody() { assertTrue(response.asString().isBlank()); }
    @Então("o erro de validação deve mencionar {string}")
    public void validationField(String field) { assertNotNull(response.jsonPath().get("errors." + field)); }
    @Então("o registro criado deve ser consultável")
    public void createdPoint() {
        int id = response.jsonPath().getInt("id");
        String name = response.jsonPath().getString("nome");
        assertTrue(response.header("Location").endsWith("/api/pontos-coleta/" + id));
        call("GET", "/api/pontos-coleta/" + id, null); expect(200); schema("ponto");
        assertEquals(name, response.jsonPath().getString("nome"));
    }
    @Então("a ocupação do ponto deve ser {word} kg")
    public void occupation(String expected) {
        call("GET", "/api/pontos-coleta/${pontoId}", null); expect(200); schema("ponto"); numberField("ocupacaoAtualKg", expected);
    }
    @Então("deve haver {int} registros no ponto")
    public void records(int count) {
        call("GET", "/api/registros-residuos?pontoColetaId=${pontoId}", null);
        expect(200); schema("registros"); assertEquals(count, response.jsonPath().getInt("totalItens"));
    }
    @Então("deve haver {int} alertas abertos de nível {string} no ponto")
    public void alerts(int count, String level) {
        call("GET", "/api/alertas-coleta?pontoColetaId=${pontoId}&resolvido=false", null);
        expect(200); schema("alertas"); assertEquals(count, response.jsonPath().getInt("totalItens"));
        if (count > 0) {
            assertEquals(level, response.jsonPath().getString("itens[0].nivel"));
            assertEquals(Integer.parseInt(variables.get("pontoId")), response.jsonPath().getInt("itens[0].pontoColetaId"));
        }
    }
    @Então("o relatório deve refletir mais {word} kg de plástico")
    public void reportDelta(String weight) {
        call("GET", "/api/relatorios-impacto/resumo", null); expect(200); schema("relatorio");
        BigDecimal expected = initialWeight.add(new BigDecimal(weight));
        assertEquals(0, expected.compareTo(decimal("totalResiduosRegistradosKg")));
        assertEquals(0, initialPlastic.add(new BigDecimal(weight)).compareTo(decimal("residuosPorTipo.Plastico")));
        assertEquals(0, expected.multiply(new BigDecimal("0.75")).setScale(2, java.math.RoundingMode.HALF_EVEN)
            .compareTo(decimal("estimativaImpactoPositivoKg")));
        assertEquals(initialActive, response.jsonPath().getInt("pontosColetaAtivos"));
        assertEquals(initialAlerts, response.jsonPath().getInt("alertasAbertos"));
    }
    @Então("a destinação criada deve ser consultável por tipo")
    public void destinationPersisted() {
        String description = response.jsonPath().getString("descricao");
        call("GET", "/api/destinacoes-residuos/9", null); expect(200); schema("destinacao");
        assertEquals(description, response.jsonPath().getString("descricao"));
    }
    @Então("a atualização do ponto deve estar persistida")
    public void updatePersisted() {
        String name = response.jsonPath().getString("nome");
        call("GET", "/api/pontos-coleta/${pontoId}", null); expect(200); schema("ponto");
        assertEquals(name, response.jsonPath().getString("nome"));
        numberField("capacidadeMaximaKg", "150");
    }
    @Então("o alerta resolvido deve estar persistido")
    public void resolvedPersisted() {
        call("GET", "/api/alertas-coleta?pontoColetaId=${pontoId}&resolvido=true", null);
        expect(200); schema("alertas"); assertEquals(1, response.jsonPath().getInt("totalItens"));
        assertTrue(response.jsonPath().getBoolean("itens[0].resolvido"));
        assertNotNull(response.jsonPath().getString("itens[0].resolvidoEm"));
    }
}
