using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Compliance.ScoreCards.Domain;
using Compliance.ScoreCards.Domain.ValueTypes;
using Compliance.ScoreCards.Implementation.Ef.Maps;

namespace Compliance.ScoreCards.Implementation.Ef
{
    public class ScoreCardsContext : DbContext
    {
        public ScoreCardsContext(string connString)
            : base(connString)
        {
            this.Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer(new ScoreCardsContextInit());
        }

        public DbSet<ScoreCard> ScoreCards { get; set; }
        public DbSet<ScoreCardResult> ScoreCardResults { get; set; }
        public DbSet<ScoreCardReview> ScoreCardReviews { get; set; }
        public DbSet<Question> Questions { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new AnswerMap());
            modelBuilder.Configurations.Add(new QuestionMap());

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public class ScoreCardsContextInit : DropCreateDatabaseIfModelChanges<ScoreCardsContext>
        {
            protected override void Seed(ScoreCardsContext context)
            {
                CreateAMScoreCard(context);
                CreateRPCScoreCard(context);
                Create3PScoreCard(context);
            }

            private void Create3PScoreCard(ScoreCardsContext context)
            {
                var scId = Guid.NewGuid();

                var thirdSC = new ScoreCard()
                {
                    Id = scId,
                    Title = "Third Party",
                    Version = 1,
                    Assertions = new List<Assertion>(),
                    UtcCreated = DateTime.UtcNow
                };

                //Disclosures and identification
                var assId1 = Guid.NewGuid();

                //Message Delivery
                var assId4 = Guid.NewGuid();

                //Consumer Requests
                var assId3 = Guid.NewGuid();

                var assert1 = new Assertion()
                {
                    Id = assId1,
                    MyScoreCardId = scId,
                    EscalationLevelOfConcern = 2,
                    Statement = "Proper Disclosures and Identification Provided",
                    PassThreshold = 3,
                    Order = 1,
                    Questions = new List<Question>() { 
                        new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId1,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector provide the following disclosure: \"This call may be monitored or recorded\"?",
                            HelpCopy = "HELP HERE",
                            Order = 1
                        },
                         new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId1,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector identify himself/herself and state that he/she is trying confirm or correct location information concerning the consumer?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 2
                        },
                         new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId1,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did steps 1 and 2 occur at the beginning of the call?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 3
                        }
                    }
                };

                var assert3 = new Assertion()
                {
                    Id = assId3,
                    MyScoreCardId = scId,
                    EscalationLevelOfConcern = 2,
                    Statement = "Collector Responded to Consumer Requests Properly",
                    PassThreshold = 2,
                    Order = 3,
                    Questions = new List<Question>() { 
                        new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId3,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 0,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 100,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector state the consumer owed a debt?",
                            HelpCopy = "HELP HERE",
                            Order = 1
                        }
                    }
                };

                var assert4 = new Assertion()
                {
                    Id = assId4,
                    MyScoreCardId = scId,
                    EscalationLevelOfConcern = 2,
                    Statement = "Proper Third Party Message Relay",
                    PassThreshold = 2,
                    Order = 2,
                    Questions = new List<Question>()
                };


                var pa4 = Guid.NewGuid();

                var qSub4 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId4,
                    MyParentAnswerId = pa4,
                    Query = "Did the collector leave their name and contact number?",
                    Weight = 1,
                    HelpCopy = "Help here",
                    Order = 2,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "Yes",
                            Percentage = 100,
                            Order = 1
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 0,
                            Order = 2
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "N/A",
                            Percentage = 0,
                            Order = 3
                        }
                    }
                };

                var qmain4 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId4,
                    Query = "Did the collector give the third party a message to deliver to the consumer?",
                    HelpCopy = "HELP HERE",
                    Weight = 1,
                    Order = 1,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = pa4,
                            Label = "Yes",
                            Percentage = 0,
                            Order = 1,
                            ChildQuestions = new List<Question>(){
                                qSub4
                            }
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 100,
                            Order = 2
                        }
                    }
                };

                var pa3 = Guid.NewGuid();
                var qSub3 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId3,
                    MyParentAnswerId = pa3,
                    Query = "Did the third party directly ask the company name?",
                    Weight = 1,
                    HelpCopy = "Help here",
                    Order = 3,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "Yes",
                            Percentage = 100,
                            Order = 1
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 0,
                            Order = 2
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "N/A",
                            Percentage = 0,
                            Order = 3
                        }
                    }
                };

                var qmain3 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId3,
                    Query = "Did the collector leave the name of the company?",
                    HelpCopy = "HELP HERE",
                    Weight = 1,
                    Order = 2,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = pa3,
                            Label = "Yes",
                            Percentage = 0,
                            Order = 1,
                            ChildQuestions = new List<Question>(){
                                qSub3
                            }
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 100,
                            Order = 2
                        }
                    }
                };

                context.ScoreCards.Add(thirdSC);

                context.SaveChanges();

                thirdSC.Assertions.Add(assert1);
                thirdSC.Assertions.Add(assert3);
                thirdSC.Assertions.Add(assert4);

                context.SaveChanges();

                context.Questions.Add(qmain4);
                context.Questions.Add(qmain3);
                context.SaveChanges();
            }

            private void CreateRPCScoreCard(ScoreCardsContext context)
            {
                var scId = Guid.NewGuid();

                var rpcSC = new ScoreCard()
                {
                    Id = scId,
                    Title = "Right Party",
                    Version = 1,
                    Assertions = new List<Assertion>(),
                    UtcCreated = DateTime.UtcNow
                };

                //Disclosures and identification
                var assId1 = Guid.NewGuid();

                //Professional Conduct
                var assId2 = Guid.NewGuid();

                //Consumer Requests
                var assId3 = Guid.NewGuid();

                //Payment Handling
                var assId4 = Guid.NewGuid();

                //Call Wrap-up
                var assId5 = Guid.NewGuid();

                var assert1 = new Assertion()
                {
                    Id = assId1,
                    MyScoreCardId = scId,
                    EscalationLevelOfConcern = 2,
                    Statement = "Proper Disclosures and Identification Provided",
                    PassThreshold = 6,
                    Order = 1,
                    Questions = new List<Question>() { 
                        new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId1,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector verify the consumer as the right party per company policy?",
                            HelpCopy = "HELP HERE",
                            Order = 1
                        },
                         new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId1,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector identify themselves and the company properly?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 2
                        },
                         new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId1,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector inform the consumer that the call was \"an attempt to collect a debt from a debt collector and that any information obtained would be used for that purpose\"?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 3
                        },
                         new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId1,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector provide the following disclosure: \"This call may be monitored or recorded\"?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 4
                        },
                         new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId1,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector provide correct account information such as creditor, original creditor if applicable, balance due, and that \"the account or claim\" was placed in our office for collection?  Or if call is a subsequent consumer contact, did the collector always state the underlying debt correctly?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 5
                        },
                         new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId1,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did all of steps 1-5 occur at the beginning of the call?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 6
                        }
                    }
                };

                var assert2 = new Assertion()
                {
                    Id = assId2,
                    MyScoreCardId = scId,
                    EscalationLevelOfConcern = 5,
                    Statement = "Call Handled in a Professional Manner",
                    PassThreshold = 4,
                    Order = 2,
                    Questions = new List<Question>(){
                        new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId2,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 0,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 100,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector contact the consumer at a time or place known to be inconvenient?",
                            HelpCopy = "HELP HERE",
                            Order = 1
                        },
                         new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId2,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 0,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 100,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector use deceptive, unfair, or misleading language (threats of legal action, untrue statements, misinforming regarding credit report impacts, etc.)?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 2
                        },
                         new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId2,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 0,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 100,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector use any harassing or abusive language (cursing, yelling, over talking, rude, disrespectful, etc.)?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 3
                        },
                         new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId2,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 0,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 100,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Was the collector in any other way unprofessional?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 4
                        }
                    }
                };


                var assert3 = new Assertion()
                {
                    Id = assId3,
                    MyScoreCardId = scId,
                    EscalationLevelOfConcern = 2,
                    Statement = "Collector Responded to Consumer Requests Properly",
                    PassThreshold = 2,
                    Order = 3,
                    Questions = new List<Question>()
                };

                var assert4 = new Assertion()
                {
                    Id = assId4,
                    MyScoreCardId = scId,
                    EscalationLevelOfConcern = 2,
                    Statement = "Payments Handled Properly",
                    PassThreshold = 2,
                    Order = 4,
                    Questions = new List<Question>()
                };


                var pa4 = Guid.NewGuid();

                var qSub4 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId4,
                    MyParentAnswerId = pa4,
                    Query = "Did the collector summarize the amount, payment due date, type of agreement, and payment method clearly?",
                    Weight = 1,
                    HelpCopy = "Help here",
                    Order = 2,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "Yes",
                            Percentage = 100,
                            Order = 1
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 0,
                            Order = 2
                        }
                    }
                };

                var qmain4 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId4,
                    Query = "Was a payment agreement reached?",
                    HelpCopy = "HELP HERE",
                    Weight = 1,
                    Order = 1,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = pa4,
                            Label = "Yes",
                            Percentage = 0,
                            Order = 1,
                            ChildQuestions = new List<Question>(){
                                qSub4
                            }
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 100,
                            Order = 2
                        }
                    }
                };

                var pa41 = Guid.NewGuid();

                var qSub41 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId4,
                    MyParentAnswerId = pa41,
                    Query = "Did the collector transfer to a payment verifier to complete the authorization?",
                    Weight = 1,
                    HelpCopy = "Help here",
                    Order = 4,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "Yes",
                            Percentage = 100,
                            Order = 1
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 0,
                            Order = 2
                        }
                    }
                };

                var qmain41 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId4,
                    Query = "Was a payment method taken internally (ACH or Credit Card)?",
                    HelpCopy = "HELP HERE",
                    Weight = 1,
                    Order = 3,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = pa41,
                            Label = "Yes",
                            Percentage = 0,
                            Order = 1,
                            ChildQuestions = new List<Question>(){
                                qSub41
                            }
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 100,
                            Order = 2
                        }
                    }
                };

                assert4.Questions.Add(qmain4);
                assert4.Questions.Add(qmain41);




                var pa5 = Guid.NewGuid();

                var qSub5 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId3,
                    MyParentAnswerId = pa5,
                    Query = "Did the collector accurately describe the debt verification process?",
                    Weight = 1,
                    HelpCopy = "Help here",
                    Order = 2,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "Yes",
                            Percentage = 100,
                            Order = 1
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 0,
                            Order = 2
                        }
                    }
                };

                var qmain5 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId3,
                    Query = "Did the consumer ask about the debt verification process?",
                    HelpCopy = "HELP HERE",
                    Weight = 1,
                    Order = 1,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = pa5,
                            Label = "Yes",
                            Percentage = 0,
                            Order = 1,
                            ChildQuestions = new List<Question>(){
                                qSub5
                            }
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 100,
                            Order = 2
                        }
                    }
                };

                var pa51 = Guid.NewGuid();

                var qSub51 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId3,
                    MyParentAnswerId = pa51,
                    Query = "Did the collector transfer the call?",
                    Weight = 1,
                    HelpCopy = "Help here",
                    Order = 4,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "Yes",
                            Percentage = 100,
                            Order = 1
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 0,
                            Order = 2
                        }
                    }
                };

                var qmain51 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId3,
                    Query = "Did the consumer ask for a manager?",
                    HelpCopy = "HELP HERE",
                    Weight = 1,
                    Order = 3,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = pa51,
                            Label = "Yes",
                            Percentage = 0,
                            Order = 1,
                            ChildQuestions = new List<Question>(){
                                qSub51
                            }
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 100,
                            Order = 2
                        }
                    }
                };

                assert3.Questions.Add(qmain5);
                assert3.Questions.Add(qmain51);

                var assert5 = new Assertion()
               {
                   Id = assId5,
                   MyScoreCardId = scId,
                   EscalationLevelOfConcern = 2,
                   Statement = "Call Ended Properly",
                   PassThreshold = 2,
                   Order = 5,
                   Questions = new List<Question>(){
                        new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId2,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector accurately note the account record when the call occurred?",
                            HelpCopy = "HELP HERE",
                            Order = 1
                        },
                         new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId2,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector end the call properly?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 2
                        }
                    }
               };






                context.ScoreCards.Add(rpcSC);

                context.SaveChanges();

                rpcSC.Assertions.Add(assert1);
                rpcSC.Assertions.Add(assert2);
                rpcSC.Assertions.Add(assert3);
                rpcSC.Assertions.Add(assert4);
                rpcSC.Assertions.Add(assert5);

                context.SaveChanges();

                //context.Questions.Add(qmain4);
                //context.Questions.Add(qmain41);
                //context.Questions.Add(qmain5);
                //context.Questions.Add(qmain51);
                //context.SaveChanges();
            }

            private void CreateAMScoreCard(ScoreCardsContext context)
            {
                var scId = Guid.Parse("BA0B0472-55B4-427C-9CC1-F254690949D7");

                var amSC = new ScoreCard()
                {
                    Id = scId,
                    Title = "Answering Machine Message",
                    Version = 1,
                    Assertions = new List<Assertion>(),
                    UtcCreated = DateTime.UtcNow
                };

                var assId1 = Guid.Parse("531B199A-9F93-46E0-9375-AC7D5F78B9BF");
                var assId2 = Guid.Parse("4FF39AF3-264B-42E1-BC31-2B01469464A4");

                var assert1 = new Assertion()
                {
                    Id = assId1,
                    MyScoreCardId = scId,
                    EscalationLevelOfConcern = 2,
                    Statement = "Scripted Message Left",
                    PassThreshold = 1,
                    Order = 1,
                    Questions = new List<Question>() { 
                        new Question(){
                            Id = Guid.NewGuid(),
                            MyAssertionId = assId1,
                            Answers = new List<Answer>(){
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "Yes",
                                    Percentage = 100,
                                    Order = 1
                                },
                                new Answer(){
                                    Id = Guid.NewGuid(),
                                    Label = "No",
                                    Percentage = 0,
                                    Order = 2
                                }
                            },
                            Weight = 1,
                            Query = "Did the collector leave the scripted message per company policy?",
                            HelpCopy = "COMPLETE SCRIPT HERE",
                            Order = 1
                        }
                    }
                };

                var assert2 = new Assertion()
                {
                    Id = assId2,
                    MyScoreCardId = scId,
                    EscalationLevelOfConcern = 2,
                    Statement = "Reasonable certainty that the message left is being delivered the right party",
                    PassThreshold = 1,
                    Order = 2,
                    Questions = new List<Question>()
                };

                var pa1 = Guid.NewGuid();
                var pa2 = Guid.NewGuid();

                var na1 = Guid.NewGuid();
                var na2 = Guid.NewGuid();

                var qSub1 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assert2.Id,
                    MyParentAnswerId = pa1,
                    Query = "Did the voicemail greeting identify the consumer by their full name?",
                    Weight = 1,
                    HelpCopy = "Help here",
                    Order = 2,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "Yes",
                            Percentage = 100,
                            Order = 1
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 0,
                            Order = 2
                        }
                    }
                };

                var qSub2 = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assert2.Id,
                    MyParentAnswerId = pa2,
                    Query = "Did the voicemail greeting identify the consumer by their full name?",
                    Weight = 1,
                    HelpCopy = "Help here",
                    Order = 2,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "Yes",
                            Percentage = 100,
                            Order = 1
                        },
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "No",
                            Percentage = 0,
                            Order = 2
                        }
                    }
                };

                var qmain = new Question()
                {
                    Id = Guid.NewGuid(),
                    MyAssertionId = assId2,
                    Query = "Which phone number was called (Be certain)?",
                    HelpCopy = "If it was the home number, it mush be the home number as identified on the work card.",
                    Weight = 1,
                    Order = 1,
                    Answers = new List<Answer>() { 
                        new Answer(){
                            Id = Guid.NewGuid(),
                            Label = "Home",
                            Percentage = 100,
                            Order = 1
                        },
                        new Answer(){
                            Id = pa1,
                            Label = "Work",
                            Percentage = 0,
                            Order = 2,
                            ChildQuestions = new List<Question>(){
                                qSub1
                            }
                        },
                        new Answer(){
                            Id = pa2,
                            Label = "Other/Unknown",
                            Percentage = 0,
                            Order = 3,
                            ChildQuestions = new List<Question>(){
                                qSub2
                            }
                        }
                    }
                };

                context.ScoreCards.Add(amSC);

                context.SaveChanges();

                amSC.Assertions.Add(assert1);
                amSC.Assertions.Add(assert2);

                context.SaveChanges();

                context.Questions.Add(qmain);
                context.SaveChanges();
            }
        }
    }
}
