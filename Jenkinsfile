pipeline {
    agent any

    environment {
        AZURE_SUBSCRIPTION_ID = 'de5a0532-73c7-453d-8d29-3d1d0fa63b84'
        AZURE_CREDENTIALS = credentials('azure-sp')
        AZURE_CLIENT_ID = "${AZURE_CREDENTIALS_USR}"
        AZURE_CLIENT_SECRET = "${AZURE_CREDENTIALS_PSW}"
        AZURE_TENANT_ID = '6e1c962e-7264-422f-bda9-e938110a5609'
        AZURE_REGISTRY_NAME = 'sit223registry'
        AZURE_CONTAINER_APP_NAME = 'datingapp-container-prod'
        AZURE_RESOURCE_GROUP = 'DatingApp-RG'
        AZURE_CONTAINER_APP_ENV = 'datingapp-env'
        AZURE_LOCATION = 'australiaeast'
        IMAGE_TAG = "v${BUILD_NUMBER}"
        IMAGE_TESTING = "sit223app-testing:${IMAGE_TAG}"
        IMAGE_STAGING = "sit223app-staging:${IMAGE_TAG}"
        IMAGE_PRO = "${AZURE_REGISTRY_NAME}.azurecr.io/sit223app:${IMAGE_TAG}"
    }

    stages {
        stage('Build') {
            steps {
                echo "############################################# 🛠️ Building App 🛠️ #############################################"
                powershell '''
                    # PART 1: Build backend
                    echo "********************** ➡️ Building Backend (.NET) **********************"
                    cd ../SIT223-Task-7.3/DatingApp.Server
                    dotnet restore
                    dotnet build -c Release

                    # PART 2: Build frontend
                    echo "********************** ➡️ Building Frontend (Angular) **********************"
                    cd ../DatingApp.Client
                    npm install
                    ng build --configuration production

                    # PART 3: Build docker container for UI tests
                    echo "********************** ➡️ Building Docker Image For Testing **********************"
                    cd ..
                    docker build --no-cache -f Docker/dockerfile.testing -t $env:IMAGE_TESTING .
                    echo "✅ Build Stage COMPLETED..."
                '''
            }
        }

        stage('Test (Unit and Selenium)') {
            steps {
                echo "######################################## 🧪 Running Tests 🧪 ########################################"
                powershell '''
                    # PART 1: Unit tests for frontend
                    echo "********************** ➡️ Running Angular unit tests **********************"
                    cd DatingApp.Client
                    npm run test -- --watch=false --browsers=ChromeHeadless
                    cd ..

                    # PART 2: Unit tests for backend
                    echo "********************** ➡️ Running .NET unit tests **********************"
                    cd BackendTests
                    dotnet test
                    cd ..

                    # PART 3: Selenium UI tests
                    echo "********************** ➡️ Starting the app in Docker for UI tests **********************"
                    docker-compose -f Docker/docker.testing.yml -p sit223apptesting up -d
                    docker logs DatingApp-staging

                    # Wait until the app is ready (max 60 seconds)
                    echo "⏳ Waiting for the app to start..."
                    powershell ./PowershellScripts/waitForApp.ps1

                    echo "********************** ➡️ Running Selenium UI tests **********************"
                    cd SeleniumTests
                    dotnet test
                    cd ..

                    echo "🧹 Stopping app after tests..."
                    docker-compose -f Docker/docker.testing.yml -p sit223apptesting down
                    echo "✅ Test Stage COMPLETED..."
                '''
            }
        }

        stage('Code Quality (SonarQube)') {
            steps {
                withSonarQubeEnv('SonarQube') { 
                    echo "######################################## 🕵️‍♂️ Running SonarQube Scans 🕵️‍♂️ ########################################"
                    powershell '''
                        # PART 1: Scan Angular project
                        echo "********************** ➡️ Scanning Angular project **********************"
                        cd DatingApp.Client
                        cmd.exe /c "sonar-scanner.bat -Dsonar.projectKey=DatingApp-Client -Dsonar.sources=src -Dsonar.exclusions=**/node_modules/**,**/*.spec.ts -Dsonar.javascript.lcov.reportPaths=coverage/lcov.info -Dsonar.host.url=\$env:SONAR_HOST_URL -Dsonar.token=\$env:SONAR_AUTH_TOKEN"
                        cd ..

                        # PART 2: Scan .NET backend project
                        echo "********************** ➡️ Scanning .NET Backend (.NET Core) project **********************"
                        cd DatingApp.Server
                        dotnet sonarscanner begin `
                            /k:"DatingApp-Server" `
                            /d:sonar.host.url=\$env:SONAR_HOST_URL `
                            /d:sonar.token=\$env:SONAR_AUTH_TOKEN
                        dotnet build --no-restore
                        dotnet sonarscanner end /d:sonar.token=\$env:SONAR_AUTH_TOKEN
                        cd ..
                        echo "✅ Code Quality Stage COMPLETED..."
                    '''
                }
            }
        }

        stage('Security (Dependency Scans)') {
            steps {
                echo "######################################## 🔒 Running Security Scans 🔒 ########################################"
                powershell '''
                    # PART 1: Frontend - Check npm package vulnerabilities
                    echo "********************** ➡️ Checking Angular frontend dependencies for vulnerabilities **********************"
                    cd DatingApp.Client
                    npm install
                    npm audit --audit-level=moderate
                    cd ..

                    # PART 2: backend - Check for vulnerable dependencies
                    echo "********************** ➡️ Checking .NET backend dependencies for vulnerabilities **********************"
                    cd DatingApp.Server
                    dotnet restore
                    dotnet list package --vulnerable
                    cd ..
                    echo "✅ Security Stage COMPLETED..."
                '''
            }
        }

        stage('Deploy (Staging)') {
            steps {
                echo "######################################## 🚀 Deploying to Staging (Docker) 🚀 ########################################"
                powershell '''
                    echo "********************** 🆕 Starting new container **********************"
                    docker build --no-cache -f Docker/dockerfile.azure -t $env:IMAGE_STAGING .
                    docker-compose -f Docker/docker.staging.yml -p sit223appstaging up -d
                    echo "🖥️ Monitor application at: http://localhost:8081"
                    sleep 30
                    echo "✅ Staging deployment completed..."
                '''
            }
        }

        stage('Release (Production)') {
            steps {
                echo "################################# 🚀 Deploying to Production (Azure) 🚀 #################################"
                powershell '''
                    echo "********************** ➡️ Logging into Azure **********************"
                    az login `
                        --service-principal `
                        --username $env:AZURE_CLIENT_ID `
                        --password $env:AZURE_CLIENT_SECRET `
                        --tenant $env:AZURE_TENANT_ID

                    echo "********************** ➡️ Logging into ACR **********************"
                    az acr login --name $env:AZURE_REGISTRY_NAME

                    echo "********************** ➡️ Building Docker image for Azure **********************"
                    docker-compose -f Docker/docker.staging.yml -p sit223appstaging down
                    docker build --no-cache -f Docker/dockerfile.azure -t $env:IMAGE_PRO .

                    echo "********************** ➡️ Pushing Docker image **********************"
                    docker push $env:IMAGE_PRO

                    echo "********************** ➡️ Updating ACR to use new image **********************"
                    az containerapp update `
                        --name $env:AZURE_CONTAINER_APP_NAME `
                        --resource-group $env:AZURE_RESOURCE_GROUP `
                        --image $env:IMAGE_PRO

                    $AppUrl = az containerapp show `
                        --name $env:AZURE_CONTAINER_APP_NAME `
                        --resource-group $env:AZURE_RESOURCE_GROUP `
                        --query properties.configuration.ingress.fqdn `
                        -o tsv

                    echo "🌍 Application is live at: https://$AppUrl"
                    echo "✅ Release State COMPLETED..."
                '''
            }
        }
    }

    post {
        success {
            echo "✅ Pipeline COMPLETED."
            emailext to: 's224899095@deakin.edu.au',
                subject: "Jenkins Pipeline - ✅ SUCCESS: ${currentBuild.fullDisplayName}",
                body: "The Jenkins pipeline for Task 7.3 Completed successfully. Please find the logs attached.",
                attachLog: true, 
                compressLog: false
        }
        failure {
            echo "❌ Pipeline FAILED."
            emailext to: 's224899095@deakin.edu.au',
                subject: "Jenkins Pipeline - ❌ FAILED",
                body: "The Jenkins pipeline for Task 7.3 Failed. Please find the logs attached.",
                attachLog: true, 
                compressLog: false
        }
    }
}
