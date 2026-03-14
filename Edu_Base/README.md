# Edu_Base

## API Endpoints

### Student progress summary
- **GET** `/api/HomeScreen/progress`  
  Returns the authenticated student's progress summary (in-progress courses count, completed lessons, average grade, paginated courses, and paginated upcoming milestones). Requires authentication.
- **Query parameters:**
  - `coursesPage` (int, default: 1)
  - `coursesPageSize` (int, default: 6)
  - `milestonesPage` (int, default: 1)
  - `milestonesPageSize` (int, default: 10)