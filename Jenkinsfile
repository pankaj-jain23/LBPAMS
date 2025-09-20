pipeline {
    agent any

    parameters {
        choice(name: 'DEPLOY_ENV', choices: ['staging', 'production'], description: 'Select the deployment environment')
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
                    if (params.DEPLOY_ENV == 'production') {
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

                    echo "Building Docker image on ${SERVER1}..."
                    ssh -i ${SSH_KEY} -o StrictHostKeyChecking=no ${SSH_USER}@${SERVER1} '
                        cd ${WORKSPACE_DIR}
                        docker build --build-arg ENVIRONMENT=${APP_ENV} -t ${IMAGE_NAME}:${IMAGE_TAG} .
                        docker save ${IMAGE_NAME}:${IMAGE_TAG} -o ${WORKSPACE_DIR}/${IMAGE_NAME}_${IMAGE_TAG}.tar
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
                    emailext(
                        subject: "Approval Needed for Build #${env.BUILD_NUMBER}",
                        body: """Hello Approver,<br><br>
                                Build #${env.BUILD_NUMBER} (${params.DEPLOY_ENV}) is waiting for your approval in Jenkins.<br>
                                Please <a href="${env.BUILD_URL}">click here</a> to approve/reject.<br><br>
                                Regards,<br>Jenkins""",
                        to: "lbpams-ap1"
                    )

                    input(
                        message: "Deploy to ${params.DEPLOY_ENV}? Approval required.",
                        ok: 'Approve',
                        submitter: 'lbpams-ap1'
                    )
                }
            }
        }

        stage('Test') {
            steps {
                echo 'Running tests...'
                // Add your .NET test commands here
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
                kubectl rollout restart deployment ${APP_LABEL}
            '

            echo "Deploying on Server2..."
            ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER2} '
                ctr -n k8s.io images import /tmp/${IMAGE_NAME}_${IMAGE_TAG}.tar
                # Re-tag image so Kubernetes finds it
                ctr -n k8s.io images tag docker.io/library/${IMAGE_NAME}:${IMAGE_TAG} ${IMAGE_NAME}:${IMAGE_TAG}
                kubectl apply -f ${KUBE_YAML}
                kubectl rollout restart deployment ${APP_LABEL}
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
