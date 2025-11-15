pipeline {
    agent any
    
    tools {
    dotnet-sdk 'dotnet8' // <-- ¡Solo agregamos "-sdk"!
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