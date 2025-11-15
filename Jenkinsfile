pipeline {
    agent any

    tools {
        dotnetsdk 'dotnet8'   // Nombre tal cual está en Global Tool Configuration
    }

    stages {

        stage('Checkout') {
            steps {
                echo 'Descargando el código desde Git-UCR...'
                checkout scm
            }
        }

        stage('Verificar .NET dentro de DEMEX') {
            steps {
                dir('DEMEX') {
                    echo "Estamos dentro de la carpeta: ${pwd()}"
                    bat 'dotnet --version'
                }
            }
        }

        stage('Restore') {
            steps {
                dir('DEMEX') {
                    bat 'dotnet restore'
                }
            }
        }

        stage('Build') {
            steps {
                dir('DEMEX') {
                    bat 'dotnet build --configuration Release'
                }
            }
        }

        stage('Test') {
    steps {
        dir('DEMEX') {
            // 1. Ejecuta las pruebas unitarias (estas siguen igual)
            bat 'dotnet test LAFABRICA.Tests/LAFABRICA.Tests.csproj'

            // 2. Ejecuta SOLO TUS pruebas de Selenium
            // El filtro buscará cualquier prueba dentro de una clase que termine en ".OrderTests"
            bat 'dotnet test LAFABRICA.UI.Test/LAFABRICA.UI.Test.csproj --filter "FullyQualifiedName~.OrderTests"'
        }
    }
}
    }

    post {
        always {
            echo 'Pipeline finalizado.'
        }
    }
}
