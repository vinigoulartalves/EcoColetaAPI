FROM maven:3.9.9-eclipse-temurin-17
WORKDIR /suite
COPY pom.xml .
RUN mvn -B -ntp dependency:go-offline
COPY src/ src/
CMD ["sh", "-c", "mvn -B -ntp clean verify; result=$?; cp -r target/. /reports/; chmod -R a+rwX /reports; exit \"$result\""]
