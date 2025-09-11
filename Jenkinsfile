pipeline {
    agent any
    environment {
        IMAGE_NAME = "lbpamsprod"
        IMAGE_TAG = "1"                              // Fixed tag
        KUBE_YAML = "/Kubernates-deployments/LBPAMS_Kubernetes.yaml"  // Correct path
        SERVER1 = "10.44.237.116"
        SERVER2 = "10.44.237.117"
        SSH_USER = "root"
        SSH_KEY = "/var/lib/jenkins/.ssh/id_ed25519_lbpams"
        WORKSPACE_DIR = "/var/lib/jenkins/workspace/LPAMS-API-PIPELINE"
    }

    stages {
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
                    echo "Copying only source code to Server1..."
                    rsync -av --exclude='*.tar' --exclude='.git' ${WORKSPACE_DIR}/ ${SSH_USER}@${SERVER1}:${WORKSPACE_DIR}/

                    echo "Building Docker image on ${SERVER1}..."
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} '
                        cd ${WORKSPACE_DIR}
                        docker build -t ${IMAGE_NAME}:${IMAGE_TAG} .
                        docker save ${IMAGE_NAME}:${IMAGE_TAG} -o ${WORKSPACE_DIR}/${IMAGE_NAME}_${IMAGE_TAG}.tar
                    '

                    echo "Copying Docker tar from Server1 to Server2..."
                    scp -i ${SSH_KEY} ${SSH_USER}@${SERVER1}:${WORKSPACE_DIR}/${IMAGE_NAME}_${IMAGE_TAG}.tar ${SSH_USER}@${SERVER2}:/tmp/

                    echo "Loading Docker image on Server2..."
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER2} "docker load -i /tmp/${IMAGE_NAME}_${IMAGE_TAG}.tar"
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
                                Build #${env.BUILD_NUMBER} is waiting for your approval in Jenkins.<br>
                                Please <a href="${env.BUILD_URL}">click here</a> to approve/reject.<br><br>
                                Regards,<br>Jenkins""",
                        to: "lbpams-ap1"
                    )

                    input(
                        message: 'Build approval required by approver',
                        ok: 'Approve',
                        submitter: 'lbpams-ap1'
                    )
                }
            }
        }

        stage('Test') {
            steps {
                echo 'Running tests...'
                // Add your test commands here
            }
        }

        stage('Deploy to Kubernetes') {
            steps {
                script {
                    sh """
                    echo "Deploying on Server1..."
                    scp -i ${SSH_KEY} ${KUBE_YAML} ${SSH_USER}@${SERVER1}:${WORKSPACE_DIR}/
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} "
                        kubectl apply -f ${WORKSPACE_DIR}/LBPAMS_Kubernetes.yaml
                        kubectl rollout restart deployment lbpams-prod
                    "

                    echo "Deploying on Server2..."
                    scp -i ${SSH_KEY} ${KUBE_YAML} ${SSH_USER}@${SERVER2}:/tmp/
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER2} "
                        kubectl apply -f /tmp/LBPAMS_Kubernetes.yaml
                        kubectl rollout restart deployment lbpams-prod
                    "

                    echo "Cleaning up Docker tar files on both servers..."
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER1} "rm -f ${WORKSPACE_DIR}/lbpamsprod_1.tar"
                    ssh -i ${SSH_KEY} ${SSH_USER}@${SERVER2} "rm -f /tmp/lbpamsprod_1.tar"
                    """
                }
            }
        }
    }

    post {
        always {
            echo "Cleaning up local temporary Docker tar..."
            sh "rm -f /tmp/lbpamsprod_1.tar"
        }
    }
}
