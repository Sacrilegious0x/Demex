pipeline {
    agent any
    
   pipeline {
    agent any

    tools {
        dotnetsdk 'dotnet8'   // nombre EXACTO como aparece en Global Tool Configuration
    }

    stages {
        stage('Restore') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build --configuration Release'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test'
            }
        }
    }
}


    stages {
        stage('Checkout') {
            steps {
                // 1. Descarga el código de la rama DEV
                echo 'Descargando el código desde Git-UCR...'
                checkout scm
            }
        }
        
        stage('Verificar .NET dentro de DEMEX') {
            steps {
                echo 'Verificando la versión de .NET...'
                
                // 2. ¡IMPORTANTE! Entramos a la carpeta de tu solución
                dir('DEMEX') {
                    // 3. Ahora SÍ, ejecutamos el comando
                    echo "Estamos dentro de la carpeta: ${pwd()}"
                    bat 'dotnet --version'
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