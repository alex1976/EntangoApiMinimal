# PowerShell script to test DocumentStore upload and download operations
# Make sure the API is running before executing this script

$baseUrl = "http://localhost:5162/"  # Adjust port if needed
$documentStoreUrl = "$baseUrl/DocumentStore"

# Get the script's directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "=== DocumentStore Upload/Download Test Script ===" -ForegroundColor Cyan
Write-Host "Base URL: $documentStoreUrl" -ForegroundColor Yellow
Write-Host "Sample Data Directory: $scriptDir" -ForegroundColor Yellow
Write-Host ""

# Display menu and get user selection
Write-Host "=== Select Operation ===" -ForegroundColor Magenta
Write-Host "1. Upload documents" -ForegroundColor White
Write-Host "2. Download documents" -ForegroundColor White
Write-Host "3. Delete documents" -ForegroundColor White
Write-Host "4. List all documents" -ForegroundColor White
Write-Host "5. Run all operations (upload, list, download, optional delete)" -ForegroundColor White
Write-Host ""

$operation = Read-Host "Enter operation number (1-5)"
Write-Host ""

# Check if sample files exist in SampleData folder
$sampleFiles = Get-ChildItem -Path $scriptDir -File -ErrorAction SilentlyContinue

if ($sampleFiles.Count -eq 0) {
    Write-Host "No files found in SampleData folder. Creating sample files..." -ForegroundColor Yellow
    
    # Create sample text file
    "This is a sample text document for testing DocumentStore upload." | Out-File -FilePath "$scriptDir\sample_document.txt" -Encoding UTF8
    
    # Create sample JSON file
    @{
        title = "Sample JSON Document"
        description = "This is a test document"
        data = @(1, 2, 3, 4, 5)
    } | ConvertTo-Json | Out-File -FilePath "$scriptDir\sample_data.json" -Encoding UTF8
    
    Write-Host "Created sample files: sample_document.txt, sample_data.json" -ForegroundColor Green
    Write-Host ""
    
    $sampleFiles = Get-ChildItem -Path $scriptDir -File
}

# Function to upload a document
function Upload-Document {
    param(
        [string]$FilePath,
        [string]$Description,
        [string]$Type,
        [string]$Category
    )
    
    Write-Host "Uploading: $FilePath" -ForegroundColor Cyan
    
    $fileInfo = Get-Item $FilePath
    $form = @{
        Document = $fileInfo
        DocumentDescription = $Description
    }
    
    if ($Type) { $form.DocumentType = $Type }
    if ($Category) { $form.DocumentCategory = $Category }
    
    try {
        $response = Invoke-RestMethod -Uri $documentStoreUrl -Method Post -Form $form -ContentType "multipart/form-data"
        Write-Host "✓ Uploaded successfully - DocumentId: $($response.documentId)" -ForegroundColor Green
        return $response
    }
    catch {
        Write-Host "✗ Upload failed: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ErrorDetails.Message) {
            Write-Host "  Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
        }
        return $null
    }
}

# Function to download a document
function Download-Document {
    param(
        [int]$DocumentId,
        [string]$OutputPath
    )
    
    Write-Host "Downloading DocumentId: $DocumentId" -ForegroundColor Cyan
    
    try {
        $downloadUrl = "$documentStoreUrl/$DocumentId/download"
        Invoke-WebRequest -Uri $downloadUrl -OutFile $OutputPath
        Write-Host "✓ Downloaded to: $OutputPath" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "✗ Download failed: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Function to list all documents
function Get-AllDocuments {
    Write-Host "Fetching all documents..." -ForegroundColor Cyan
    
    try {
        $documents = Invoke-RestMethod -Uri $documentStoreUrl -Method Get
        Write-Host "✓ Found $($documents.Count) document(s)" -ForegroundColor Green
        return $documents
    }
    catch {
        Write-Host "✗ Failed to fetch documents: $($_.Exception.Message)" -ForegroundColor Red
        return @()
    }
}

# Function to delete a document
function Remove-Document {
    param([int]$DocumentId)
    
    Write-Host "Deleting DocumentId: $DocumentId" -ForegroundColor Cyan
    
    try {
        Invoke-RestMethod -Uri "$documentStoreUrl/$DocumentId" -Method Delete
        Write-Host "✓ Document deleted successfully" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "✗ Delete failed: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Main execution based on user selection
switch ($operation) {
    "1" {
        # Upload documents
        Write-Host "=== Upload Documents ===" -ForegroundColor Magenta
        Write-Host ""
        
        # Check if sample files exist
        $sampleFiles = Get-ChildItem -Path $scriptDir -File -Exclude "*.ps1"
        if ($sampleFiles.Count -eq 0) {
            Write-Host "No files found in SampleData folder. Creating sample files..." -ForegroundColor Yellow
            
            "This is a sample text document for testing DocumentStore upload." | Out-File -FilePath "$scriptDir\sample_document.txt" -Encoding UTF8
            @{
                title = "Sample JSON Document"
                description = "This is a test document"
                data = @(1, 2, 3, 4, 5)
            } | ConvertTo-Json | Out-File -FilePath "$scriptDir\sample_data.json" -Encoding UTF8
            
            Write-Host "Created sample files: sample_document.txt, sample_data.json" -ForegroundColor Green
            Write-Host ""
            $sampleFiles = Get-ChildItem -Path $scriptDir -File -Exclude "*.ps1"
        }
        
        foreach ($file in $sampleFiles) {
            $extension = $file.Extension.ToLower()
            $type = switch ($extension) {
                ".txt" { "Text" }
                ".json" { "JSON" }
                ".pdf" { "PDF" }
                ".docx" { "Word" }
                ".xlsx" { "Excel" }
                default { "Other" }
            }
            
            Upload-Document -FilePath $file.FullName `
                           -Description "Test upload: $($file.Name)" `
                           -Type $type `
                           -Category "Sample"
            Write-Host ""
        }
    }
    
    "2" {
        # Download documents
        Write-Host "=== Download Documents ===" -ForegroundColor Magenta
        Write-Host ""
        
        $allDocuments = Get-AllDocuments
        if ($allDocuments.Count -eq 0) {
            Write-Host "No documents found in database." -ForegroundColor Yellow
            exit
        }
        
        Write-Host "Available documents:" -ForegroundColor Yellow
        $allDocuments | Format-Table DocumentId, DocumentDescription, DocumentType, DocumentExtension, DocumentSize -AutoSize
        Write-Host ""
        
        $downloadChoice = Read-Host "Download (A)ll documents or (S)pecific ID? (A/S)"
        
        $downloadDir = Join-Path $scriptDir "Downloads"
        if (-not (Test-Path $downloadDir)) {
            New-Item -Path $downloadDir -ItemType Directory | Out-Null
            Write-Host "Created downloads directory: $downloadDir" -ForegroundColor Yellow
            Write-Host ""
        }
        
        if ($downloadChoice -eq 'A' -or $downloadChoice -eq 'a') {
            foreach ($doc in $allDocuments) {
                $outputFileName = "downloaded_$($doc.documentId)_$($doc.documentDescription.Replace(' ', '_'))$($doc.documentExtension)"
                $outputPath = Join-Path $downloadDir $outputFileName
                Download-Document -DocumentId $doc.documentId -OutputPath $outputPath
                Write-Host ""
            }
        }
        elseif ($downloadChoice -eq 'S' -or $downloadChoice -eq 's') {
            $docId = Read-Host "Enter DocumentId to download"
            $doc = $allDocuments | Where-Object { $_.documentId -eq [int]$docId }
            if ($doc) {
                $outputFileName = "downloaded_$($doc.documentId)_$($doc.documentDescription.Replace(' ', '_'))$($doc.documentExtension)"
                $outputPath = Join-Path $downloadDir $outputFileName
                Download-Document -DocumentId $doc.documentId -OutputPath $outputPath
            }
            else {
                Write-Host "Document not found." -ForegroundColor Red
            }
        }
        
        Write-Host ""
        Write-Host "Download location: $downloadDir" -ForegroundColor Yellow
    }
    
    "3" {
        # Delete documents
        Write-Host "=== Delete Documents ===" -ForegroundColor Magenta
        Write-Host ""
        
        $allDocuments = Get-AllDocuments
        if ($allDocuments.Count -eq 0) {
            Write-Host "No documents found in database." -ForegroundColor Yellow
            exit
        }
        
        Write-Host "Available documents:" -ForegroundColor Yellow
        $allDocuments | Format-Table DocumentId, DocumentDescription, DocumentType, DocumentExtension -AutoSize
        Write-Host ""
        
        $deleteChoice = Read-Host "Delete (A)ll documents or (S)pecific ID? (A/S)"
        
        if ($deleteChoice -eq 'A' -or $deleteChoice -eq 'a') {
            $confirm = Read-Host "Are you sure you want to delete ALL documents? (y/n)"
            if ($confirm -eq 'y') {
                foreach ($doc in $allDocuments) {
                    Remove-Document -DocumentId $doc.documentId
                    Write-Host ""
                }
                Write-Host "All documents deleted." -ForegroundColor Green
            }
            else {
                Write-Host "Deletion cancelled." -ForegroundColor Yellow
            }
        }
        elseif ($deleteChoice -eq 'S' -or $deleteChoice -eq 's') {
            $docId = Read-Host "Enter DocumentId to delete"
            $doc = $allDocuments | Where-Object { $_.documentId -eq [int]$docId }
            if ($doc) {
                $confirm = Read-Host "Delete document '$($doc.documentDescription)'? (y/n)"
                if ($confirm -eq 'y') {
                    Remove-Document -DocumentId $doc.documentId
                }
                else {
                    Write-Host "Deletion cancelled." -ForegroundColor Yellow
                }
            }
            else {
                Write-Host "Document not found." -ForegroundColor Red
            }
        }
    }
    
    "4" {
        # List all documents
        Write-Host "=== List All Documents ===" -ForegroundColor Magenta
        Write-Host ""
        
        $allDocuments = Get-AllDocuments
        if ($allDocuments.Count -gt 0) {
            $allDocuments | Format-Table DocumentId, DocumentDescription, DocumentType, DocumentCategory, DocumentExtension, DocumentSize -AutoSize
            Write-Host ""
            Write-Host "Total documents: $($allDocuments.Count)" -ForegroundColor Yellow
        }
        else {
            Write-Host "No documents found in database." -ForegroundColor Yellow
        }
    }
    
    "5" {
        # Run all operations
        Write-Host "=== Running All Operations ===" -ForegroundColor Magenta
        Write-Host ""
        
        # Upload
        Write-Host "=== Step 1: Upload Sample Documents ===" -ForegroundColor Magenta
        Write-Host ""
        
        $uploadedDocuments = @()
        $sampleFiles = Get-ChildItem -Path $scriptDir -File -Exclude "*.ps1"
        
        if ($sampleFiles.Count -eq 0) {
            Write-Host "No files found. Creating sample files..." -ForegroundColor Yellow
            "This is a sample text document for testing DocumentStore upload." | Out-File -FilePath "$scriptDir\sample_document.txt" -Encoding UTF8
            @{
                title = "Sample JSON Document"
                description = "This is a test document"
                data = @(1, 2, 3, 4, 5)
            } | ConvertTo-Json | Out-File -FilePath "$scriptDir\sample_data.json" -Encoding UTF8
            $sampleFiles = Get-ChildItem -Path $scriptDir -File -Exclude "*.ps1"
        }
        
        foreach ($file in $sampleFiles) {
            $extension = $file.Extension.ToLower()
            $type = switch ($extension) {
                ".txt" { "Text" }
                ".json" { "JSON" }
                ".pdf" { "PDF" }
                ".docx" { "Word" }
                ".xlsx" { "Excel" }
                default { "Other" }
            }
            
            $result = Upload-Document -FilePath $file.FullName `
                                       -Description "Test upload: $($file.Name)" `
                                       -Type $type `
                                       -Category "Sample"
            
            if ($result) {
                $uploadedDocuments += $result
            }
            Write-Host ""
        }
        
        if ($uploadedDocuments.Count -eq 0) {
            Write-Host "No documents were uploaded. Exiting." -ForegroundColor Red
            exit
        }
        
        # List
        Write-Host "=== Step 2: List All Documents ===" -ForegroundColor Magenta
        Write-Host ""
        
        $allDocuments = Get-AllDocuments
        if ($allDocuments.Count -gt 0) {
            $allDocuments | Format-Table DocumentId, DocumentDescription, DocumentType, DocumentCategory, DocumentExtension, DocumentSize -AutoSize
        }
        Write-Host ""
        
        # Download
        Write-Host "=== Step 3: Download Documents ===" -ForegroundColor Magenta
        Write-Host ""
        
        $downloadDir = Join-Path $scriptDir "Downloads"
        if (-not (Test-Path $downloadDir)) {
            New-Item -Path $downloadDir -ItemType Directory | Out-Null
            Write-Host "Created downloads directory: $downloadDir" -ForegroundColor Yellow
            Write-Host ""
        }
        
        foreach ($doc in $uploadedDocuments) {
            $outputFileName = "downloaded_$($doc.documentId)_$($doc.documentDescription.Replace(' ', '_'))$($doc.documentExtension)"
            $outputPath = Join-Path $downloadDir $outputFileName
            Download-Document -DocumentId $doc.documentId -OutputPath $outputPath
            Write-Host ""
        }
        
        Write-Host "=== Step 4: Verify Downloaded Files ===" -ForegroundColor Magenta
        Write-Host ""
        
        $downloadedFiles = Get-ChildItem -Path $downloadDir -File
        Write-Host "Downloaded files in $downloadDir" -ForegroundColor Yellow
        $downloadedFiles | Format-Table Name, Length, LastWriteTime -AutoSize
        Write-Host ""
        
        # Optional cleanup
        Write-Host "=== Step 5: Clean Up (Optional) ===" -ForegroundColor Magenta
        Write-Host ""
        
        $cleanup = Read-Host "Do you want to delete the uploaded documents from the database? (y/n)"
        
        if ($cleanup -eq 'y') {
            foreach ($doc in $uploadedDocuments) {
                Remove-Document -DocumentId $doc.documentId
                Write-Host ""
            }
            Write-Host "Cleanup completed." -ForegroundColor Green
        } else {
            Write-Host "Skipped cleanup. Documents remain in the database." -ForegroundColor Yellow
        }
        
        Write-Host ""
        Write-Host "=== Test Script Completed ===" -ForegroundColor Cyan
        Write-Host "Summary:" -ForegroundColor Yellow
        Write-Host "  - Uploaded: $($uploadedDocuments.Count) document(s)" -ForegroundColor White
        Write-Host "  - Downloaded: $($downloadedFiles.Count) file(s)" -ForegroundColor White
        Write-Host "  - Download location: $downloadDir" -ForegroundColor White
    }
    
    default {
        Write-Host "Invalid operation selected. Please run the script again and choose 1-5." -ForegroundColor Red
    }
}
