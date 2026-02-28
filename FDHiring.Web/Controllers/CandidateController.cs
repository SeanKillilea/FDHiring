using Microsoft.AspNetCore.Mvc;
using FDHiring.Data.Repositories;
using FDHiring.Core.Models;
using FDHiring.Web.Helpers;

namespace FDHiring.Web.Controllers
{
    public class CandidateController : Controller
    {
        private readonly CandidateRepository _candidates;
        private readonly CandidateFileRepository _files;
        private readonly PositionRepository _positions;
        private readonly AgencyRepository _agencies;
        private readonly WorkflowRepository _workflow;
        private readonly WorkflowStepRepository _workflowSteps;
        private readonly UserRepository _users;
        private readonly InterviewRepository _interviews;
        private readonly InterviewTypeRepository _interviewTypes;
        private readonly IWebHostEnvironment _env;
        private readonly InterviewQuestionRepository _interviewQuestions;
        private readonly InterviewAnswerRepository _interviewAnswers;

        public CandidateController(
            CandidateRepository candidates,
            CandidateFileRepository files,
            PositionRepository positions,
            AgencyRepository agencies,
            WorkflowRepository workflow,
            WorkflowStepRepository workflowSteps,
            UserRepository users,
            InterviewRepository interviews,
            InterviewTypeRepository interviewTypes,
            InterviewQuestionRepository interviewQuestions,
            InterviewAnswerRepository interviewAnswers,
            IWebHostEnvironment env)
        {
            _candidates = candidates;
            _files = files;
            _positions = positions;
            _agencies = agencies;
            _workflow = workflow;          
            _workflowSteps = workflowSteps;
            _users = users;
            _interviews = interviews; 
            _interviewTypes = interviewTypes;
            _interviewQuestions = interviewQuestions;
            _interviewAnswers = interviewAnswers;
            _env = env;
        }

        // ───── SEARCH ─────

        public async Task<IActionResult> Search(string? q)
        {
            ViewData["Title"] = "Candidate Search";
            ViewData["SidebarActive"] = "candidates";

            var candidateId = HttpContext.Session.GetCandidateId();
            if (candidateId > 0)
            {
                ViewData["CandidateId"] = candidateId;
                ViewData["Candidate"] = await _candidates.GetByIdAsync(candidateId);
            }

            var searchState = HttpContext.Session.GetSearchState();

            if (q != null)
            {
                ViewData["SearchName"] = q;
                HttpContext.Session.SetSearchState(q, null, searchState.current, searchState.active, searchState.wouldHire);
            }
            else
            {
                ViewData["SearchName"] = searchState.name;
            }

            ViewData["SearchActive"] = searchState.active;
            ViewData["SearchCurrent"] = searchState.current;
            ViewData["SearchWouldHire"] = searchState.wouldHire;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchJson(string? name, bool? active = null, bool? current = null, bool? wouldHire = null)
        {
            HttpContext.Session.SetSearchState(name, null, current == true, active, wouldHire == true);

            var all = await _candidates.SearchAsync(null, null, null, null, null, null);
            var results = await _candidates.SearchAsync(name, null, null, active, current, wouldHire);

            return Json(new
            {
                results = results.Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.Phone,
                    c.LinkedIn,
                    c.PositionName,
                    c.Notes,
                    c.Active,
                    c.IsCurrent,
                    c.WouldHire,
                    c.PhotoPath
                }),
                total = all.Count()
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetJson(int id)
        {
            var c = await _candidates.GetByIdAsync(id);
            if (c == null) return NotFound();

            return Json(new
            {
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.Phone,
                c.LinkedIn,
                c.PositionName,
                c.Notes,
                c.Active,
                c.IsCurrent,
                c.WouldHire,
                c.PhotoPath
            });
        }

        [HttpGet]
        public IActionResult SetCandidate(int? id)
        {
            if (id.HasValue)
                HttpContext.Session.SetCandidateId(id.Value);
            else
                HttpContext.Session.Remove(SessionKeys.CandidateId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Ok();

            return RedirectToAction("Search");
        }

        // ───── PAGES ─────

        public async Task<IActionResult> Add()
        {
            ViewData["Title"] = "Add Candidate";
            ViewData["SidebarActive"] = "add";
            ViewData["Positions"] = await _positions.GetAllAsync();
            ViewData["Agencies"] = await _agencies.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(string firstName, string lastName,
            string email, string? phone, string? linkedIn, int positionId, int agencyId,
            string? dateFound, string? notes, bool active, bool wouldHire, bool isCurrent)
        {
            var candidate = new Candidate
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                LinkedIn = linkedIn,
                PositionId = positionId,
                AgencyId = agencyId,
                DateFound = string.IsNullOrEmpty(dateFound) ? DateTime.Today : DateTime.Parse(dateFound),
                Notes = notes,
                Active = active,
                WouldHire = wouldHire,
                IsCurrent = isCurrent,
                LastUpdatedByUserId = HttpContext.Session.GetUserId()
            };

            var newId = await _candidates.InsertAsync(candidate);

            // Set as active candidate in session
            HttpContext.Session.SetCandidateId(newId);

            TempData["ToastMessage"] = "Candidate created";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("Files");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit Candidate";
            ViewData["SidebarActive"] = "candidates";

            var candidate = await _candidates.GetByIdAsync(id);
            if (candidate == null) return RedirectToAction("Search");

            ViewData["Candidate"] = candidate;
            ViewData["CandidateId"] = id;
            ViewData["Positions"] = await _positions.GetAllAsync();
            ViewData["Agencies"] = await _agencies.GetAllAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string firstName, string lastName,
            string email, string? phone, string? linkedIn, int positionId, int agencyId,
            string? dateFound, string? notes, bool active, bool wouldHire, bool isCurrent)
        {
            var candidate = await _candidates.GetByIdAsync(id);
            if (candidate == null) return RedirectToAction("Search");

            candidate.FirstName = firstName;
            candidate.LastName = lastName;
            candidate.Email = email;
            candidate.Phone = phone;
            candidate.LinkedIn = linkedIn;
            candidate.PositionId = positionId;
            candidate.AgencyId = agencyId;
            candidate.DateFound = string.IsNullOrEmpty(dateFound) ? DateTime.Today : DateTime.Parse(dateFound);
            candidate.Notes = notes;
            candidate.Active = active;
            candidate.WouldHire = wouldHire;
            candidate.IsCurrent = isCurrent;
            candidate.LastUpdatedByUserId = HttpContext.Session.GetUserId();

            await _candidates.UpdateAsync(candidate);

            TempData["ToastMessage"] = "Candidate updated";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("Edit", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            await _candidates.DeleteAsync(id);

            // Clear session if this was the active candidate
            var sessionCandidateId = HttpContext.Session.GetCandidateId();
            if (sessionCandidateId == id)
                HttpContext.Session.Remove(SessionKeys.CandidateId);

            TempData["ToastMessage"] = "Candidate deleted";
            TempData["ToastVariant"] = "danger";
            return RedirectToAction("Search");
        }


        // ───── FILES PAGE ─────

        public async Task<IActionResult> Files(int? fileId)
        {
            ViewData["Title"] = "Candidate Files";
            ViewData["SidebarActive"] = "files";

            var candidateId = HttpContext.Session.GetCandidateId();
            if (candidateId > 0)
            {
                ViewData["CandidateId"] = candidateId;
                ViewData["Candidate"] = await _candidates.GetByIdAsync(candidateId);
                ViewData["Files"] = (await _files.GetByCandidateIdAsync(candidateId)).ToList();

                if (fileId.HasValue)
                {
                    var selected = await _files.GetByIdAsync(fileId.Value);
                    if (selected != null && selected.CandidateId == candidateId)
                        ViewData["SelectedFile"] = selected;
                }
            }

            return View();
        }

        // ───── FILE ACTIONS ─────

        [HttpPost]
        public async Task<IActionResult> UploadFile(int candidateId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided");

            var userId = HttpContext.Session.GetUserId();
            if (userId == 0)
                return BadRequest("No user in session");

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "candidates", candidateId.ToString());
            Directory.CreateDirectory(uploadsDir);

            var safeFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";
            var physicalPath = Path.Combine(uploadsDir, safeFileName);
            var webPath = $"/uploads/candidates/{candidateId}/{safeFileName}";

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var candidateFile = new CandidateFile
            {
                CandidateId = candidateId,
                FileName = file.FileName,
                FilePath = webPath,
                FileSize = file.Length,
                IsUserPicture = false,
                UploadedByUserId = userId
            };

            await _files.InsertAsync(candidateFile);

            // Return JSON for the AJAX drag-drop upload
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFile(int id, string? fileDescription, bool isUserPicture)
        {
            var f = await _files.GetByIdAsync(id);
            if (f == null) return NotFound();

            f.FileDescription = fileDescription;
            f.IsUserPicture = isUserPicture;
            await _files.UpdateAsync(f);

            if (isUserPicture)
                await _files.SetProfilePictureAsync(id, f.CandidateId);

            TempData["ToastMessage"] = "File updated";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("Files", new { fileId = id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var f = await _files.GetByIdAsync(id);
            if (f == null) return NotFound();

            var physicalPath = Path.Combine(_env.WebRootPath, f.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(physicalPath))
                System.IO.File.Delete(physicalPath);

            await _files.DeleteAsync(id);

            TempData["ToastMessage"] = "File deleted";
            TempData["ToastVariant"] = "danger";
            return RedirectToAction("Files");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var f = await _files.GetByIdAsync(id);
            if (f == null) return NotFound();

            var physicalPath = Path.Combine(_env.WebRootPath, f.FilePath.TrimStart('/'));
            if (!System.IO.File.Exists(physicalPath))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(physicalPath);
            return File(bytes, "application/octet-stream", f.FileName);
        }


        // ───── WORKFLOW ─────

        public async Task<IActionResult> Workflow(int? stepId)
        {
            ViewData["Title"] = "Candidate Workflow";
            ViewData["SidebarActive"] = "workflow";

            var candidateId = HttpContext.Session.GetCandidateId();
            if (candidateId <= 0)
                return View(); // no candidate selected

            var candidate = await _candidates.GetByIdAsync(candidateId);
            if (candidate == null)
                return View();

            ViewData["CandidateId"] = candidateId;
            ViewData["Candidate"] = candidate;

            var positionId = candidate.PositionId;
            ViewData["PositionId"] = positionId;

            // Workflow steps for this candidate + position
            var steps = (await _workflow.GetByCandidateAndPositionAsync(candidateId, positionId)).ToList();
            ViewData["WorkflowSteps"] = steps;

            // Template steps for the add dropdown
            var templates = (await _workflowSteps.GetByPositionIdAsync(positionId)).ToList();
            ViewData["TemplateSteps"] = templates;

            // Users for owner dropdown
            var users = (await _users.GetAllAsync()).ToList();
            ViewData["Users"] = users;

            // Selected step for editing
            if (stepId.HasValue)
            {
                var selected = steps.FirstOrDefault(s => s.Id == stepId.Value);
                if (selected != null)
                    ViewData["SelectedStep"] = selected;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddWorkflowStep(int candidateId, int positionId,
            string stepName, string? owner)
        {
            // Determine next step order
            var existing = await _workflow.GetByCandidateAndPositionAsync(candidateId, positionId);
            var maxOrder = existing.Any() ? existing.Max(s => s.StepOrder) : 0;

            var step = new Workflow
            {
                CandidateId = candidateId,
                PositionId = positionId,
                StepName = stepName,
                Owner = owner,
                StepOrder = maxOrder + 1,
                Complete = false
            };

            var newId = await _workflow.InsertAsync(step);

            TempData["ToastMessage"] = "Step added";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("Workflow", new { stepId = newId });
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddAllWorkflowSteps(int candidateId, int positionId)
        {
            await _workflow.DeleteByCandidateAndPositionAsync(candidateId, positionId);
            await _workflow.InsertAllStepsForPositionAsync(candidateId, positionId);

            TempData["ToastMessage"] = "All template steps added";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("Workflow");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWorkflowStep(int id, string stepName, string? owner,
                string? startDate, bool complete)
        {
            var step = await _workflow.GetByIdAsync(id);
            if (step == null) return RedirectToAction("Workflow");

            step.StepName = stepName;
            step.Owner = owner;
            step.StartDate = string.IsNullOrEmpty(startDate) ? null : DateTime.Parse(startDate);
            step.Complete = complete;

            await _workflow.UpdateAsync(step);

            TempData["ToastMessage"] = "Step updated";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("Workflow", new { stepId = id });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleWorkflowComplete(int id, bool complete)
        {
            await _workflow.ToggleCompleteAsync(id, complete);

            TempData["ToastMessage"] = complete ? "Step completed" : "Step reopened";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("Workflow");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWorkflowStep(int id)
        {
            var step = await _workflow.GetByIdAsync(id);
            if (step == null) return RedirectToAction("Workflow");

            await _workflow.DeleteAsync(id);

            TempData["ToastMessage"] = "Step deleted";
            TempData["ToastVariant"] = "danger";
            return RedirectToAction("Workflow");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllWorkflowSteps(int candidateId, int positionId)
        {
            await _workflow.DeleteByCandidateAndPositionAsync(candidateId, positionId);

            TempData["ToastMessage"] = "All steps deleted";
            TempData["ToastVariant"] = "danger";
            return RedirectToAction("Workflow");
        }

        [HttpPost]
        public async Task<IActionResult> ReorderWorkflowSteps([FromBody] List<ReorderItem> items)
        {
            foreach (var item in items)
            {
                await _workflow.UpdateStepOrderAsync(item.Id, item.StepOrder);
            }
            return Ok();
        }

        public class ReorderItem
        {
            public int Id { get; set; }
            public int StepOrder { get; set; }
        }


        // ───── INTERVIEWS ─────
        public async Task<IActionResult> Interviews(int? interviewId)
        {
            ViewData["Title"] = "Candidate Interviews";
            ViewData["SidebarActive"] = "interview";

            var candidateId = HttpContext.Session.GetCandidateId();
            if (candidateId <= 0)
                return View();

            var candidate = await _candidates.GetByIdAsync(candidateId);
            if (candidate == null)
                return View();

            ViewData["CandidateId"] = candidateId;
            ViewData["Candidate"] = candidate;

            var positionId = candidate.PositionId;
            ViewData["PositionId"] = positionId;

            // Interviews for this candidate + position
            var interviews = (await _interviews.GetByCandidateAndPositionAsync(candidateId, positionId)).ToList();
            ViewData["Interviews"] = interviews;

            // Dropdowns
            var types = (await _interviewTypes.GetAllAsync()).ToList();
            ViewData["InterviewTypes"] = types;

            var users = (await _users.GetAllAsync()).ToList();
            ViewData["Users"] = users;

            // Selected interview for editing
            if (interviewId.HasValue)
            {
                var selected = interviews.FirstOrDefault(i => i.Id == interviewId.Value);
                if (selected != null)
                    ViewData["SelectedInterview"] = selected;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddInterview(int candidateId, int positionId,
            int interviewTypeId, string owner, string? scheduledDate, string? notes)
        {
            var interview = new Interview
            {
                CandidateId = candidateId,
                PositionId = positionId,
                InterviewTypeId = interviewTypeId,
                Owner = owner,
                ScheduledDate = string.IsNullOrEmpty(scheduledDate) ? null : DateTime.Parse(scheduledDate),
                CandidateGo = false,
                Notes = notes
            };

            var newId = await _interviews.InsertAsync(interview);

            TempData["ToastMessage"] = "Interview added";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("Interviews", new { interviewId = newId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInterview(int id, int interviewTypeId, string owner,
            string? scheduledDate, bool candidateGo, string? notes)
        {
            var interview = await _interviews.GetByIdAsync(id);
            if (interview == null) return RedirectToAction("Interviews");

            interview.InterviewTypeId = interviewTypeId;
            interview.Owner = owner;
            interview.ScheduledDate = string.IsNullOrEmpty(scheduledDate) ? null : DateTime.Parse(scheduledDate);
            interview.CandidateGo = candidateGo;
            interview.Notes = notes;

            await _interviews.UpdateAsync(interview);

            TempData["ToastMessage"] = "Interview updated";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("Interviews", new { interviewId = id });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleInterviewGo(int id, bool candidateGo)
        {
            await _interviews.ToggleGoAsync(id, candidateGo);

            TempData["ToastMessage"] = candidateGo ? "Marked as Go" : "Marked as No Go";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("Interviews");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteInterview(int id)
        {
            await _interviews.DeleteAsync(id);

            TempData["ToastMessage"] = "Interview deleted";
            TempData["ToastVariant"] = "danger";
            return RedirectToAction("Interviews");
        }



        // ───── INTERVIEW QUESTION / ANSWER ─────

        public async Task<IActionResult> InterviewDetail(int id)
        {
            ViewData["Title"] = "Interview Detail";
            ViewData["SidebarActive"] = "interview";

            var candidateId = HttpContext.Session.GetCandidateId();
            if (candidateId <= 0)
                return RedirectToAction("Interviews");

            var candidate = await _candidates.GetByIdAsync(candidateId);
            if (candidate == null)
                return RedirectToAction("Interviews");

            var interview = await _interviews.GetByIdAsync(id);
            if (interview == null)
                return RedirectToAction("Interviews");

            ViewData["CandidateId"] = candidateId;
            ViewData["Candidate"] = candidate;
            ViewData["Interview"] = interview;

            // Load answers for this interview
            var answers = (await _interviewAnswers.GetByInterviewIdAsync(id)).ToList();

            // If no answers exist yet, auto-load from template questions
            if (!answers.Any())
            {
                var userName = HttpContext.Session.GetUserFullName();
                await _interviewAnswers.LoadQuestionsForInterviewAsync(
                    id, interview.PositionId, interview.InterviewTypeId, userName);
                answers = (await _interviewAnswers.GetByInterviewIdAsync(id)).ToList();
            }

            ViewData["Answers"] = answers;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInterviewDetail(int interviewId, bool candidateGo,
            string? notes, int[] answerIds, string[] answers)
        {
            // Update interview go + notes
            var interview = await _interviews.GetByIdAsync(interviewId);
            if (interview == null) return RedirectToAction("Interviews");

            interview.CandidateGo = candidateGo;
            interview.Notes = notes;
            await _interviews.UpdateAsync(interview);

            // Update all answers
            if (answerIds != null && answers != null)
            {
                for (int i = 0; i < answerIds.Length && i < answers.Length; i++)
                {
                    var answer = await _interviewAnswers.GetByIdAsync(answerIds[i]);
                    if (answer != null)
                    {
                        answer.Answer = answers[i];
                        await _interviewAnswers.UpdateAsync(answer);
                    }
                }
            }

            TempData["ToastMessage"] = "Interview saved";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("InterviewDetail", new { id = interviewId });
        }

        [HttpPost]
        public async Task<IActionResult> AddInterviewQuestion(int interviewId, string question)
        {
            var interview = await _interviews.GetByIdAsync(interviewId);
            if (interview == null) return RedirectToAction("Interviews");

            // Add as ad-hoc question to the template table
            var existing = await _interviewAnswers.GetByInterviewIdAsync(interviewId);
            var maxOrder = existing.Any() ? existing.Max(a => a.SortOrder) : 0;

            var q = new InterviewQuestion
            {
                PositionId = interview.PositionId,
                InterviewTypeId = interview.InterviewTypeId,
                Question = question,
                SortOrder = maxOrder + 1
            };
            var questionId = await _interviewQuestions.InsertAsync(q);

            // Add answer row for this interview
            var userName = HttpContext.Session.GetUserFullName();
            var a = new InterviewAnswer
            {
                InterviewId = interviewId,
                InterviewQuestionId = questionId,
                Answer = null,
                AnsweredByUser = userName
            };
            await _interviewAnswers.InsertAsync(a);

            TempData["ToastMessage"] = "Question added";
            TempData["ToastVariant"] = "success";
            return RedirectToAction("InterviewDetail", new { id = interviewId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteInterviewAnswer(int id, int interviewId)
        {
            await _interviewAnswers.DeleteAsync(id);

            TempData["ToastMessage"] = "Question removed";
            TempData["ToastVariant"] = "danger";
            return RedirectToAction("InterviewDetail", new { id = interviewId });
        }

        [HttpPost]
        public async Task<IActionResult> ReorderAnswers([FromBody] List<ReorderItem> items)
        {
            foreach (var item in items)
            {
                await _interviewAnswers.UpdateSortOrderAsync(item.Id, item.StepOrder);
            }
            return Ok();
        }





    }
}