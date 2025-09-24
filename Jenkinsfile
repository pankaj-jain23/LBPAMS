pipeline {
    agent any

    parameters {
        choice(name: 'DEPLOY_ENV', choices: ['Staging', 'Production'], description: 'Select the deployment environment')
    }

    environment {
        SSH_USER = "root"
        SSH_KEY = "/var/lib/jenkins/.ssh/id_ed25519_lbpams"
        WORKSPACE_DIR = "/var/lib/jenkins/workspace/LPAMS-API-PIPELINE"
    }

    stages {
        stage('Set Environment Variables') {
            steps {
                script {
                    if (params.DEPLOY_ENV == 'Production') {
                        env.IMAGE_NAME = "lbpamsprod"
                        env.IMAGE_TAG = "1"
                        env.KUBE_YAML = "/Kubernates-deployments/LBPAMS_Kubernetes.yaml"
                        env.SERVER1 = "10.43.250.211"
                        env.SERVER2 = "10.43.250.212"
                        env.APP_ENV = "Production"
                        env.APP_LABEL = "lbpams-prod"
                    } else {
                        env.IMAGE_NAME = "lbpamsstag"
                        env.IMAGE_TAG = "1"
                        env.KUBE_YAML = "/Kubernates-deployments/LBPAMS_kubernates_stag.yaml"
                        env.SERVER1 = "10.43.250.211"
                        env.SERVER2 = "10.43.250.212"
                        env.APP_ENV = "Staging"
                        env.APP_LABEL = "lbpams-stag"
                    }
                }
            }
        }

        stage('Checkout') {
            steps {
                git branch: 'uptomaster',
                    url: 'https://github.com/pankaj-jain23/LBPAMS.git',
                    credentialsId: 'ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIJsOkNK+PNfxziz/RVUye7nUxVFNkug8IkMIDRJsUZ+u serviceplus.pb@gmail.com'
            }
        }

stage('Build Docker Image on Server1') {
    steps {
        script {
            sh """
            echo "Copying source code to Server1..."
            rsync -av -e "ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no" --exclude='*.tar' --exclude='.git' ${WORKSPACE_DIR}/ ${SSH_USER}@${SERVER1}:${WORKSPACE_DIR}/

            echo "Building Docker image on ${SERVER1} for environment: ${APP_ENV}..."
            ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${SSH_USER}@${SERVER1} '
                cd ${WORKSPACE_DIR}
                # Force rebuild without cache to ensure latest appsettings are used
                docker build --no-cache --build-arg ENVIRONMENT=${APP_ENV} -t ${IMAGE_NAME}:${IMAGE_TAG} .
                docker save ${IMAGE_NAME}:${IMAGE_TAG} -o ${WORKSPACE_DIR}/${IMAGE_NAME}_${IMAGE_TAG}.tar

                # Print the appsettings.json inside the image
                docker run --rm ${IMAGE_NAME}:${IMAGE_TAG} cat /app/appsettings.json
            '

            echo "Copying Docker tar from Server1 to Server2..."
            scp -i ${SSH_KEY} -o StrictHostKeyChecking=no ${SSH_USER}@${SERVER1}:${WORKSPACE_DIR}/${IMAGE_NAME}_${IMAGE_TAG}.tar ${SSH_USER}@${SERVER2}:/tmp/
            """
        }
    }
}


      stage('Approval Required') {
    steps {
        script {
            def approvers = ['lbpams-ap1','lbpams-ap2','lbpams-ap3']

            // Extra safety: error if someone else tries to approve
            // if (!approvers.contains(env.JENKINS_USER_ID)) {
            //     error "You are not authorized to approve this build."
            // }

            input(
                message: "Deploy to ${params.DEPLOY_ENV}? Approval required.",
                ok: 'Approve',
                submitter: approvers.join(',')
            )
        }
    }
}


        stage('Test') {
            steps {
                echo 'Running tests...'
            }
        }

        stage('Deploy to Kubernetes') {
            steps {
                script {
                    sh """
                    echo "Deploying on Server1..."
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} '
                        ctr -n k8s.io images import ${WORKSPACE_DIR}/${IMAGE_NAME}_${IMAGE_TAG}.tar
                        kubectl apply -f ${KUBE_YAML}
                        kubectl rollout restart deployment ${APP_LABEL} -n default
                    '

                    echo "Deploying on Server2..."
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER2} '
                        # Ensure tar file is in /tmp
                        if [ -f /tmp/${IMAGE_NAME}_${IMAGE_TAG}.tar ]; then
                            ctr -n k8s.io images import /tmp/${IMAGE_NAME}_${IMAGE_TAG}.tar
                        fi

                        # Apply deployment and restart pods
                        kubectl apply -f ${KUBE_YAML} -n default
                        kubectl rollout restart deployment ${APP_LABEL} -n default
                    '

                    echo "Cleaning up Docker tar files on both servers..."
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} "rm -f ${WORKSPACE_DIR}/${IMAGE_NAME}_${IMAGE_TAG}.tar"
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER2} "rm -f /tmp/${IMAGE_NAME}_${IMAGE_TAG}.tar"
                    """
                }
            }
        }
    }

    post {
        always {
            echo "Cleaning up local temporary Docker tar..."
            sh "rm -f /tmp/${IMAGE_NAME}_${IMAGE_TAG}.tar"
        }
    }
}
