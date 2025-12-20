# ============================================
# Script để xóa Docker containers, images và volumes
# ============================================

Write-Host "=== DỪNG VÀ XÓA CONTAINERS TỪ DOCKER COMPOSE ===" -ForegroundColor Yellow
docker-compose -f docker-compose.yml -f docker-compose.override.yml down -v

Write-Host "`n=== XÓA TẤT CẢ CONTAINERS (đã dừng) ===" -ForegroundColor Yellow
docker container prune -f

Write-Host "`n=== XÓA TẤT CẢ IMAGES (không được sử dụng) ===" -ForegroundColor Yellow
docker image prune -a -f

Write-Host "`n=== XÓA TẤT CẢ VOLUMES (không được sử dụng) ===" -ForegroundColor Yellow
docker volume prune -f

Write-Host "`n=== XÓA TẤT CẢ NETWORKS (không được sử dụng) ===" -ForegroundColor Yellow
docker network prune -f

Write-Host "`n=== HOÀN TẤT! ===" -ForegroundColor Green

