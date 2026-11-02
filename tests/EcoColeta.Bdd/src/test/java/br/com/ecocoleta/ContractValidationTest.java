package br.com.ecocoleta;

import org.junit.jupiter.api.Test;
import static io.restassured.module.jsv.JsonSchemaValidator.matchesJsonSchemaInClasspath;
import static org.hamcrest.MatcherAssert.assertThat;
import static org.junit.jupiter.api.Assertions.assertThrows;

/** Proves that contract validation actually rejects breaking changes without needing a server. */
class ContractValidationTest {
    private static final String VALID = "{\"id\":1,\"tipoResiduo\":\"Plastico\",\"descricao\":\"Reciclagem\",\"instrucoesDescarte\":\"Separar e lavar\",\"reciclavel\":true,\"riscoAmbiental\":\"Baixo\"}";
    @Test void acceptsValidContract() { assertThat(VALID, matchesJsonSchemaInClasspath("schemas/destinacao.json")); }
    @Test void rejectsMissingRequiredField() {
        assertThrows(AssertionError.class, () -> assertThat(VALID.replace("\"id\":1,", ""), matchesJsonSchemaInClasspath("schemas/destinacao.json")));
    }
    @Test void rejectsWrongType() {
        assertThrows(AssertionError.class, () -> assertThat(VALID.replace("\"id\":1", "\"id\":\"1\""), matchesJsonSchemaInClasspath("schemas/destinacao.json")));
    }
    @Test void rejectsUnknownEnumValue() {
        assertThrows(AssertionError.class, () -> assertThat(VALID.replace("Plastico", "Desconhecido"), matchesJsonSchemaInClasspath("schemas/destinacao.json")));
    }
    @Test void rejectsUndocumentedField() {
        assertThrows(AssertionError.class, () -> assertThat(VALID.replace("\"id\":1", "\"id\":1,\"novoCampo\":42"), matchesJsonSchemaInClasspath("schemas/destinacao.json")));
    }
}
