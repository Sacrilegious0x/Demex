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
                    bat 'dotnet test'
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
